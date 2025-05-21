using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación común del servicio de autenticación
    /// </summary>
    public class CommonAuthService : IAuthService
    {
        private readonly IAuthLogic _authLogic;
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public CommonAuthService(
            DbContext dbContext,
            IConfiguration configuration,
            IAuthLogic authLogic,
            string tenantId = null)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authLogic = authLogic;
            _tenantId = tenantId;
        }

        /// <summary>
        /// Autentica a un usuario con sus credenciales
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<string> AuthenticateAsync(string email, string password)
        {
            // Buscar usuario por email
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            // Verificar contraseña
            if (!_authLogic.VerifyPassword(password, user.Password, user.PasswordSalt))
                return null;

            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("userType", ((int)user.Role).ToString()),
                new Claim("tenantId", user.TenantId ?? "central")
            };
            
            // Agregar branchId al token si el usuario es de tipo Branch
            if (user.Role == Enums.UserType.Branch && user.BranchId.HasValue)
            {
                claims.Add(new Claim("branchId", user.BranchId.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Duration"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Información del usuario</returns>
        public async Task<UserDTO> GetUserInfoAsync(int userId)
        {
            var user = await _dbContext.Set<User>().FindAsync(userId);
            if (user == null)
                return null;

            return new UserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                TenantId = _tenantId,
                UserType = user.Role
            };
        }

        /// <summary>
        /// Verifica si un token JWT es válido
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>true si el token es válido, false en caso contrario</returns>
        public bool ValidateToken(string token)
        {
            return _authLogic.ValidateToken(token);
        }
    }
}
