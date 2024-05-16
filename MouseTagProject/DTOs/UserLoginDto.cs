using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class UserLoginDto
    {
        [Required, EmailAddress, MinLength(8, ErrorMessage = "Email must be at least 8 chapters long.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
