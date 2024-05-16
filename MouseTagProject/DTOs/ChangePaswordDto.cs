using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class ChangePaswordDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
