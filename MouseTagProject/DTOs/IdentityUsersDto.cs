using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class IdentityUsersDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
