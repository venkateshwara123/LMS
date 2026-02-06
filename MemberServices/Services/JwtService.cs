using MemberServices.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MemberServices.Models;
using System.Security.Cryptography;

namespace MemberServices.Services
{
    public class JwtService: IJwtService
    {
        public string GenerateToken(Users user)
        {
            byte[] SecretBytes = new byte[64];
            using (var random=RandomNumberGenerator.Create())
            {
                random.GetBytes(SecretBytes);
            }
            string SecretKey=Convert.ToBase64String(SecretBytes);
                var claims = new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(SecretKey));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials:
                    new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
