using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TriviaGame.Api.Services.Interfaces;

namespace TriviaGame.Api.Services
{
    /// <summary>
    /// Servicio para la generacion de tokens JWT
    /// </summary> <summary>
    /// 
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IConfiguration _config;

        public AuthTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(int userId, string gmail, bool isActive)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, gmail),
                new Claim("isActive", isActive.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpireMinutes"]!)
                ),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
