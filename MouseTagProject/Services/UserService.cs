using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MouseTagProject.Context;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using MouseTagProject.Repository;
using Org.BouncyCastle.Crypto;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ToDoListProject.Services;

namespace MouseTagProject.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;
        private readonly IJwtService _jwtService;
        private readonly MouseTagProjectContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly INote _noteRepository;

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IJwtService jwtService, MouseTagProjectContext context, ICurrentUserService currentUser, INote noteRepository)
        {
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _jwtService = jwtService;
            _context = context;
            _currentUser = currentUser;
            _noteRepository = noteRepository;
        }

        public async Task<UserResponseDto> RegisterUserAsync(UserRegisterDto user)
        {
            if (await CheckIfUserMailIs(user.Email) == true) { throw new InvalidOperationException("Toks vartotojo El.paštas jau yra!"); };

            var identityUser = new ApplicationUser() { UserName = user.Email, Email = user.Email };

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            await _userManager.AddToRoleAsync(identityUser, user.Role);

            if (result.Succeeded)
            {
                return new UserResponseDto()
                {
                    Message = "User created successfully.",
                    IsSuccess = true,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new UserResponseDto()
            {
                Message = "User did not created.",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserResponseDto> LoginUserAsync(UserLoginDto user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return new UserResponseDto()
                {
                    Message = "User did not found.",
                    IsSuccess = false,
                };
            }

            var result = await _userManager.CheckPasswordAsync(identityUser, user.Password);

            if (!result)
            {
                return new UserResponseDto()
                {
                    Message = "Invalid password.",
                    IsSuccess = false,
                };
            }

            var token = await _jwtService.GetToken(identityUser);

            return new UserResponseDto()
            {
                Message = token.TokenString,
                IsSuccess = true,
                ExpiredDate = token.Token.ValidTo
            };
        }

        public async Task<List<IdentityUsersDto>> GetIdentityUsersAsync()
        {
            var usersInfo = await _userManager.Users.ToListAsync();
            List<IdentityUsersDto> users = new List<IdentityUsersDto>();
            usersInfo.ForEach(x => users.Add(new IdentityUsersDto()
            {
                Email = x.Email,
                Role = _userManager.GetRolesAsync(x).Result[0]
            }));
            if(usersInfo != null)
            {
                return users;
            }
            else
            {
                throw new InvalidOperationException("Vartotojų nerasta!");
            }
           
        }

        public async Task<bool> RemoveUsersAsync(List<UsersDeleteDto> users)
        {
            
            foreach (var x in users)
            {  
                if(x.Email == "admin@xplicity.com") {
                    throw new InvalidOperationException("Negalima ištrinti pagrindinio admin!");
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(x.Email);
                    var userCandidates = _context.Candidates.Where(c => user == c.Recruiter).ToList().Select(c => { c.Recruiter = null; return c; }).ToList();
                    _context.UpdateRange(userCandidates);
                    await _context.SaveChangesAsync();
                    var userRemoved = await _userManager.DeleteAsync(user);
                    if (!userRemoved.Succeeded)
                    {
                        return false;
                    }
                }
                

            }

            return true;
        }

        public async Task<bool> RemoveUserBaseAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user.UserName == "admin@xplicity.com") throw new InvalidOperationException("Šio vartotojo ištrynimas negalimas");

            var userCandidates = _context.Candidates.Where(c => user == c.Recruiter).ToList().Select(c => { c.Recruiter = null; return c; }).ToList();

            _context.UpdateRange(userCandidates);
            await _context.SaveChangesAsync();
            var userRemoved = await _userManager.DeleteAsync(user);

            if (userRemoved.Succeeded)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveUserAsync(string useremail, string jwtEncodedString)
        {
            jwtEncodedString = jwtEncodedString.Replace("\"", "");
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            var role = token.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
            var email = token.Claims.First(c => c.Type == "Email").Value;

            if (useremail != email)
            {
                if (role == "SuperAdmin")
                {
                    await _noteRepository.DeleteByEmail(useremail);
                    var result = await RemoveUserBaseAsync(useremail);
                    if (result)
                    {
                        return true;
                    }
                    else { return false; }
                }
                else if (role == "Admin")
                {
                    var userRole = await GetUserRoleByMailAsync(useremail);
                    Console.WriteLine(userRole);
                    if (userRole == "Admin" || userRole == "SuperAdmin")
                    {
                        throw new InvalidOperationException("Šio vartotojo jums negalima pašalinti!");
                    }
                    else if (userRole == "User")
                    {
                        await _noteRepository.DeleteByEmail(useremail);
                        var result = await RemoveUserBaseAsync(useremail);
                        if (result)
                        {
                            return true;
                        }
                        else { return false; }
                    }
                    else
                    {
                        throw new InvalidOperationException("Klaida pašalinant!");
                    }
                }
                else
                {
                    return false;
                }
            }
            else if(useremail == email)
            {
                throw new InvalidOperationException("Savęs negalima pašalinti!");
            }
            else
            {
                throw new InvalidOperationException("Įvyko klaida pašalinant!");
            }
        }

        public async Task<ApplicationUser> GetUserProfileAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user;
        }

        public async Task<bool> UpdateUserAsync(string email, IdentityUsersDto identityUsersDto, string jwtEncodedString)
        {
            if(email != identityUsersDto.Email)
            {
                if (await CheckIfUserMailIs(identityUsersDto.Email) == true) { throw new InvalidOperationException("Toks vartotojo El.paštas jau yra!"); };
            }
            jwtEncodedString = jwtEncodedString.Replace("\"", "");
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            var changerrole = token.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
            var user = await _userManager.FindByEmailAsync(email);

            if(email != null)
            {
                var Role = _userManager.GetRolesAsync(user).Result[0];
                if (changerrole == "SuperAdmin")
                {
                    List<string> listRoles = new List<string>();
                    listRoles.Add(Role);
                    IEnumerable<string> roles = listRoles;
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRoleAsync(user, identityUsersDto.Role);
                    await _userManager.SetEmailAsync(user, identityUsersDto.Email);
                    await _userManager.SetUserNameAsync(user, identityUsersDto.Email);
                    return true;
                }
                else if(changerrole == "Admin")
                {
                    if (Role == "Admin" || Role == "SuperAdmin") {
                        throw new InvalidOperationException("Šio vartotojo redaguoti jums negalima!");
                    }
                    else if(Role == "User")
                    {
                        List<string> listRoles = new List<string>();
                        listRoles.Add(Role);
                        IEnumerable<string> roles = listRoles;
                        await _userManager.RemoveFromRolesAsync(user, roles);
                        await _userManager.AddToRoleAsync(user, identityUsersDto.Role);
                        await _userManager.SetEmailAsync(user, identityUsersDto.Email);
                        await _userManager.SetUserNameAsync(user, identityUsersDto.Email);
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Įvyko klaida keičiant duomenis!");
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Įvyko klaida keičiant duomenis!");
                }
               
            }
            else
            {
                throw new InvalidOperationException("Įvyko klaida keičiant duomenis!");
            }
           
        }

        public async Task<bool> ChangePasswordPanelAsync(UserChangePasswordDto userChangePasswordDto, string jwtEncodedString)
        {
            jwtEncodedString = jwtEncodedString.Replace("\"", "");
            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            var changerrole = token.Claims.First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
            var user = await _userManager.FindByEmailAsync(userChangePasswordDto.Email);
            var userRole = await GetUserRoleByMailAsync(user.Email);
            if (userChangePasswordDto.Email != null)
            {
                if(changerrole == "SuperAdmin")
                {
                    var userToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, userToken, userChangePasswordDto.Password);

                    return true;
                }else if(changerrole == "Admin")
                {
                    if(userRole == "Admin" || userRole == "SuperAdmin")
                    {
                        throw new InvalidOperationException("Jums negalima keistį slaptažodį šiam vartotojui!");
                    }
                    else if (userRole == "User")
                    {
                        var userToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, userToken, userChangePasswordDto.Password);

                        return true;
                    }
                    else
                    {
                       throw new InvalidOperationException("Įvyko klaida keičiant slaptažodį!");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Įvyko klaida keičiant slaptažodį!");
                }
            }
            else
            {
                throw new InvalidOperationException("Įvyko klaida keičiant slaptažodį!");
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePaswordDto changePaswordDto)
        {
            var user = await _userManager.FindByEmailAsync(changePaswordDto.Email);

            var result = await _userManager.ChangePasswordAsync(user, changePaswordDto.OldPassword, changePaswordDto.ConfirmPassword);
            if (!result.Succeeded) { throw new InvalidOperationException("Neteisingas dabartinis slaptažodis!"); }

            return result;
        }

        public async Task<bool> ChangeUserMailAsync(string email, UserChangeMailDto userChangeMailDto)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var result = await _userManager.CheckPasswordAsync(user, userChangeMailDto.Password);
            if (!result)
            {
                throw new InvalidOperationException("Neteisingas slaptažodis!");
            }

            if (await CheckIfUserMailIs(userChangeMailDto.Email) == true) { throw new InvalidOperationException("Toks vartotojo El.paštas jau yra!"); };

            if (email != null)
            {
                await _userManager.SetEmailAsync(user, userChangeMailDto.Email);
                await _userManager.SetUserNameAsync(user, userChangeMailDto.Email);

                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var res = regex.IsMatch(forgotPasswordDto.Email);
            if (regex.IsMatch(forgotPasswordDto.Email))
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var param = new Dictionary<string, string?>
                {
                {"token", token },
                {"email", forgotPasswordDto.Email }
                };
                    var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
                    var message = new Message(new string[] { user.Email }, "Slaptažodžio pakeitimo nuoroda", callback);
                    _emailSender.SendEmail(message);
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Toks el.paštas nerastas!");
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("Blogas El.pašto formatas!");
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user != null)
            {
                await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<string> GetUserRoleByMailAsync(string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            var role = _userManager.GetRolesAsync(user).Result[0];

            return role;
        }

        public async Task<bool> CheckIfUserMailIs(string mail)
        {
            var user = await _userManager.FindByEmailAsync(mail);
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }    
}
