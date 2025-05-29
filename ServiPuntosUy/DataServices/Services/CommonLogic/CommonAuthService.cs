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
        private readonly ILoyaltyService _loyaltyService;

        public CommonAuthService(
            DbContext dbContext,
            IConfiguration configuration,
            IAuthLogic authLogic,
            ILoyaltyService loyaltyService = null,
            string tenantId = null)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authLogic = authLogic;
            _loyaltyService = loyaltyService;
            _tenantId = tenantId;
        }

        /// <summary>
        /// Autentica a un usuario con sus credenciales
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<UserSessionDTO?> AuthenticateAsync(string email, string password)
        {
            // Buscar usuario por email
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            // Verificar contraseña
            if (!_authLogic.VerifyPassword(password, user.Password, user.PasswordSalt))
                return null;

            // Verificar expiración de puntos solo para usuarios finales
            if (user.Role == Enums.UserType.EndUser && _loyaltyService != null)
            {
                try
                {
                    // Verificar expiración de puntos usando el servicio inyectado
                    await _loyaltyService.CheckPointsExpirationAsync(user.Id);
                }
                catch (Exception ex)
                {
                    // Logging del error pero permitir que el login continúe
                    Console.WriteLine($"Error checking point expiration: {ex.Message}");
                }
            }

            // Actualizar la fecha del último login (siempre)
            user.LastLoginDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim("email", user.Email),
                new Claim("userType", ((int)user.Role).ToString()),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim("name", user.Name),
                new Claim("userId", user.Id.ToString()),
                new Claim("lastLoginDate", user.LastLoginDate.ToString("o"))
            };

            // Agregar branchId al token si el usuario es de tipo Branch
            if (user.Role == Enums.UserType.Branch)
            {
                claims.Add(new Claim("branchId", user.BranchId.ToString()));
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
            var returnToken = tokenHandler.WriteToken(token);
            return new(returnToken);
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
