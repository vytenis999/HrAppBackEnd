using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MouseTagProject.DTOs;
using MouseTagProject.Interfaces;
using MouseTagProject.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MouseTagProject.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        public JwtService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<JwtTokenDto> GetToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = GetClaims(user, userRoles);

            var token = GenerateToken(claims);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new JwtTokenDto { Token = token, TokenString = tokenString };

        }

        private JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtConfig:Key"]));

            var token = new JwtSecurityToken(
                issuer: _config["JwtConfig:Issuer"],
                audience: _config["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private List<Claim> GetClaims(ApplicationUser user, IList<string> userRoles)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim("Email", user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
    }
}
