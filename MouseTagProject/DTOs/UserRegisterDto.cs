using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class UserRegisterDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
