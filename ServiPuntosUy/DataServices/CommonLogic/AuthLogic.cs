using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.CommonLogic
{
    public class AuthLogic : IAuthLogic
    {
        private readonly IConfiguration _config;

        public AuthLogic(IConfiguration config)
        {
            _config = config;
        }

        public Tuple<string, DateTime> GenerateToken(UserDTO user, string? currentTenant = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                // new Claim(ClaimTypes.Name, user.Name),
                // new Claim(ClaimTypes.Role, user.Role?.Id.ToString() ?? "undefined"),
                new Claim("currentTenant", currentTenant ?? string.Empty),
            };
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:MinutesTokenLifeTime"]));

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: credentials);

            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenData = new Tuple<string, DateTime>(stringToken, expires);
            return tokenData;
        }
    }
}
