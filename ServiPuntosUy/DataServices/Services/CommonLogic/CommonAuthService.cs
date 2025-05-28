using System.Text;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using System.Security.Claims;
using ServiPuntosUy.Models.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services.CommonLogic;

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
        private readonly IGenericRepository<DAO.Models.Central.User> _userRepository;
        private readonly IGenericRepository<DAO.Models.Central.Tenant> _tenantRepository;

        public CommonAuthService(
            DbContext dbContext,
            IConfiguration configuration,
            IAuthLogic authLogic,
            IGenericRepository<DAO.Models.Central.User> userRepository,
            IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
            string tenantId = null
        )
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authLogic = authLogic;
            _tenantId = tenantId;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
        }

        /// Genera un token JWT para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="role">Tipo de usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <returns>Token JWT</returns>
        public async Task<UserSessionDTO> GenerateJwtToken(int userId, string email, string name, UserType role, int? tenantId, int? branchId)
        {
            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim("email", email),
                new Claim("userType", ((int)role).ToString()),
                new Claim("tenantId", tenantId.ToString()),
                new Claim("name", name),
                new Claim("userId", userId.ToString())
            };

            // Agregar branchId al token si el usuario es de tipo Branch
            if (role == Enums.UserType.Branch)
            {
                claims.Add(new Claim("branchId", branchId.ToString()));
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

            return await GenerateJwtToken(
                user.Id,
                user.Email,
                user.Name,
                user.Role,
                user.TenantId ?? null,
                user.BranchId ?? null
            );
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

        /// <summary>
        /// Registra a un usuario
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <returns>Usuario registrado</returns>
        public async Task<UserSessionDTO> Signup(string email, string password, string name, int tenantId)
        {
            var authLogic = new AuthLogic(_configuration);
            string salt;
            var passwordHash = authLogic.HashPassword(password, out salt);

            var existingUser = await _userRepository.GetQueryable().FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null) {
                throw new Exception($"Ya existe un usuario con el email {email}");
            }

            var existingTenant = await _tenantRepository.GetQueryable().FirstOrDefaultAsync(t => t.Id == tenantId);
            if (existingTenant == null) {
                throw new Exception($"El tenant {tenantId} no existe");
            }

            var newUser = new User
            {
                Email = email,
                Name = name,
                TenantId = tenantId,
                Role = UserType.EndUser,
                IsVerified = false,
                NotificationsEnabled = true,
                LastLoginDate = DateTime.UtcNow,
                PointBalance = 0,
                Password = passwordHash,
                PasswordSalt = salt,
            };

            var createdUser = await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return await GenerateJwtToken(
                createdUser.Id,
                createdUser.Email,
                createdUser.Name,
                createdUser.Role,
                createdUser.TenantId,
                null
            );
        }
    }
}
