using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using MouseTagProject.Repository;
using NETCore.MailKit.Core;
using System.IdentityModel.Tokens.Jwt;
using ToDoListProject.Services;
using System.IdentityModel.Tokens;
using System.Runtime.Intrinsics.X86;

namespace MouseTagProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly INote _noteRepository;

        public AuthController(IUserService userService, IEmailSender emailSender, INote noteRepository)
        {
            _userService = userService;
            _noteRepository = noteRepository;
        }

        [Authorize(Roles = "Admin, SuperAdmin"), HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(userDto);
                if (result.IsSuccess == true)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUserAsync(UserLoginDto user)
        {
            try
            {
                var result = await _userService.LoginUserAsync(user);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin"), HttpGet]
        public async Task<IActionResult> GetIdentityUsersAsync()
        {
            try
            {
                var userInfo = await _userService.GetIdentityUsersAsync();
                if(userInfo != null)
                {
                    return Ok(userInfo);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin, SuperAdmin"),HttpPost("RemoveList/{jwtEncodedString}")]
        public async Task<IActionResult> RemoveUsersAsync(List<UsersDeleteDto> users, string jwtEncodedString)
        {
            try
            {
                //await _noteRepository.DeleteByEmailList(user1);
                //var result = await _userService.RemoveUsersAsync(user1);
                foreach (var x in users)
                {
                    var result = await _userService.RemoveUserAsync(x.Email, jwtEncodedString);
                    if (result == false)
                    {
                        return BadRequest();
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin"),HttpDelete("Remove/{useremail}/{jwtEncodedString}")]
        public async Task<IActionResult> RemoveUserAsync(string useremail, string jwtEncodedString)
        {
            try
            {
                var result = await _userService.RemoveUserAsync(useremail, jwtEncodedString);
                if (result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin, SuperAdmin"), HttpPut("Update/{email}/{jwtEncodedString}")]
        public async Task<IActionResult> UpdateUserAsync(string email, IdentityUsersDto identityUsersDto, string jwtEncodedString)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(email, identityUsersDto, jwtEncodedString);
                if (result)
                {
                    return Ok(result);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize, HttpPut("ChangeMailUser/{email}")]
        public async Task<IActionResult> ChangeUserMailAsync(string email, UserChangeMailDto userChangeMailDto)
        {
            try
            {
                var result = await _userService.ChangeUserMailAsync(email, userChangeMailDto);
                if (result)
                {
                    return Ok(result);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize, HttpPost("ChangePasswordUser/{jwtEncodedString}")]
        public async Task<IActionResult> ChangePasswordPanelAsync(UserChangePasswordDto userChangePasswordDto, string jwtEncodedString)
        {
            try
            {
                var result = await _userService.ChangePasswordPanelAsync(userChangePasswordDto, jwtEncodedString);
                if (result)
                {
                    return Ok(result);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize, HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePaswordDto changePaswordDto)
        {
            try
            {
                var result = await _userService.ChangePasswordAsync(changePaswordDto);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var result = await _userService.ForgotPasswordAsync(forgotPasswordDto);
                if (result == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var result = await _userService.ResetPasswordAsync(resetPasswordDto);
                if (result)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
