using MouseTagProject.DTOs;
using MouseTagProject.Models;

namespace MouseTagProject.Interfaces
{
    public interface IJwtService
    {
        Task<JwtTokenDto> GetToken(ApplicationUser user);
    }
}
