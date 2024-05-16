using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MouseTagProject.DTOs;
using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUserAsync(UserRegisterDto user);
        Task<UserResponseDto> LoginUserAsync(UserLoginDto user);
        Task<List<IdentityUsersDto>> GetIdentityUsersAsync();
        Task<ApplicationUser> GetUserProfileAsync(string id);
        Task<bool> RemoveUserBaseAsync(string email);
        Task<bool> RemoveUserAsync(string useremail, string jwtEncodedString);
        Task<bool> RemoveUsersAsync(List<UsersDeleteDto> users);
        Task<bool> UpdateUserAsync(string email, IdentityUsersDto identityUsersDto, string jwtEncodedString);
        Task<IdentityResult> ChangePasswordAsync(ChangePaswordDto changePaswordDto);
        Task<bool> ChangePasswordPanelAsync(UserChangePasswordDto userChangePasswordDto, string jwtEncodedString);
        Task<bool> ChangeUserMailAsync(string email, UserChangeMailDto userChangeMailDto);
        Task<bool> ForgotPasswordAsync([FromBody] ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<string> GetUserRoleByMailAsync(string mail);
    }
}
