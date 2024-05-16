using System.IdentityModel.Tokens.Jwt;

namespace MouseTagProject.DTOs
{
    public class JwtTokenDto
    {
        public JwtSecurityToken Token { get; set; }
        public string TokenString { get; set; }
    }
}
