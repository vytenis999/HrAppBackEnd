using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class ForgotPasswordDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? ClientURI { get; set; }
    }
}
