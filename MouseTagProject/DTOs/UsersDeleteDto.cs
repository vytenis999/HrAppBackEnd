using System.ComponentModel.DataAnnotations;

namespace MouseTagProject.DTOs
{
    public class UsersDeleteDto
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
