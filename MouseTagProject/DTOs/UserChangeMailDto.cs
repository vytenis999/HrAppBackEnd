using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class UserChangeMailDto
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

    }
}
