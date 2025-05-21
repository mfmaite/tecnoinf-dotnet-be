using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación de la lógica de autenticación común a todos los servicios
    /// </summary>
    public class AuthLogic : IAuthLogic
    {
        private readonly IConfiguration _configuration;
        
        public AuthLogic(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        /// <summary>
        /// Genera un hash para una contraseña
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="passwordSalt">Salt generado</param>
        /// <returns>Hash de la contraseña</returns>
        public string HashPassword(string password, out string passwordSalt)
        {
            // Generar un salt aleatorio
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            passwordSalt = Convert.ToBase64String(saltBytes);
            
            // Generar el hash
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        
        /// <summary>
        /// Verifica si una contraseña coincide con un hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="passwordHash">Hash de la contraseña</param>
        /// <param name="passwordSalt">Salt utilizado para generar el hash</param>
        /// <returns>true si la contraseña coincide, false en caso contrario</returns>
        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            // Generar el hash con la contraseña y el salt
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password + passwordSalt);
                var hashBytes = sha256.ComputeHash(passwordBytes);
                var computedHash = Convert.ToBase64String(hashBytes);
                
                return computedHash == passwordHash;
            }
        }
        
        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>true si el token es válido, false en caso contrario</returns>
        public bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Obtiene los claims de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Claims del token</returns>
        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Enumerable.Empty<Claim>();
            }
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.Claims;
            }
            catch
            {
                return Enumerable.Empty<Claim>();
            }
        }
        
        /// <summary>
        /// Obtiene un claim específico de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="claimType">Tipo de claim</param>
        /// <returns>Valor del claim, o null si no existe</returns>
        public string GetTokenClaim(string token, string claimType)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(claimType))
            {
                return null;
            }
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
                return claim?.Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
