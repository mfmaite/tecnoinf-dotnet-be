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
using Microsoft.AspNetCore.Http;

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
        private readonly ILoyaltyService _loyaltyService;

        public CommonAuthService(
            DbContext dbContext,
            IConfiguration configuration,
            IAuthLogic authLogic,
            IGenericRepository<DAO.Models.Central.User> userRepository,
            IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
            ILoyaltyService loyaltyService = null,
            string tenantId = null)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authLogic = authLogic;
            _loyaltyService = loyaltyService;
            _tenantId = tenantId;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="role">Tipo de usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="googleId">ID de Google del usuario (opcional)</param>
        /// <returns>Token JWT</returns>
        public async Task<UserSessionDTO> GenerateJwtToken(int userId, string email, string name, UserType role, int? tenantId, int? branchId, string googleId = null)
        {
            // Generar token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim("email", email),
                new Claim("userType", ((int)role).ToString()),
                new Claim("tenantId", tenantId.ToString()),
                new Claim("name", name),
                new Claim("userId", userId.ToString())
            };

            // Agregar branchId al token si el usuario es de tipo Branch
            if (role == UserType.Branch)
            {
                claims.Add(new Claim("branchId", branchId.ToString()));
            }

            // Agregar googleId al token si está disponible
            if (!string.IsNullOrEmpty(googleId))
            {
                claims.Add(new Claim("googleId", googleId));
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
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<UserSessionDTO?> AuthenticateAsync(string email, string password, HttpContext httpContext)
        {
            // Obtener tenantId y userType del contexto HTTP
            string? tenantIdStr = httpContext.Items["CurrentTenant"] as string;
            int? tenantId = !string.IsNullOrEmpty(tenantIdStr) && int.TryParse(tenantIdStr, out int tid) ? tid : null;
            UserType userType = (UserType)(httpContext.Items["UserType"] ?? UserType.EndUser);

            // Buscar usuario por email, tenantId y rol
            var query = _dbContext.Set<User>().Where(u => u.Email == email && u.TenantId == tenantId && u.Role == userType);

            var user = await query.FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }

            // Verificar contraseña
            if (!_authLogic.VerifyPassword(password, user.Password, user.PasswordSalt))
            {
                return null;
            }

            // Verificar expiración de puntos solo para usuarios finales
            if (user.Role == UserType.EndUser && _loyaltyService != null)
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

            // Generate token
            return await GenerateJwtToken(
                user.Id,
                user.Email,
                user.Name,
                user.Role,
                user.TenantId ?? null,
                user.BranchId ?? null,
                user.GoogleId
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
            {
                throw new ArgumentException($"No existe un usuario con ID {userId}");
            }

            return new UserDTO
            {
                Id = user.Id,
                TenantId = _tenantId,
                Email = user.Email,
                Name = user.Name,
                UserType = user.Role,
                BranchId = user.BranchId,
                IsVerified = user.IsVerified,
                PointBalance = user.PointBalance,
                NotificationsEnabled = user.NotificationsEnabled,
                GoogleId = user.GoogleId
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
        /// Autentica a un usuario con Google
        /// </summary>
        /// <param name="idToken">Token de ID de Google</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="googleId">ID de Google del usuario</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<UserSessionDTO?> AuthenticateWithGoogleAsync(
            string idToken,
            string email,
            string name,
            string googleId,
            HttpContext httpContext)
        {
            // Obtener tenantId y userType del contexto HTTP
            string? tenantIdStr = httpContext.Items["CurrentTenant"] as string;
            if (string.IsNullOrEmpty(tenantIdStr) || !int.TryParse(tenantIdStr, out int tenantId))
            {
                throw new Exception("No se pudo determinar el tenant para el login con Google");
            }

            UserType userType = (UserType)(httpContext.Items["UserType"] ?? UserType.EndUser);

            // Verify if the user exists by GoogleId
            var user = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.GoogleId == googleId && u.TenantId == tenantId);

            // If user doesn't exist, try to find by email
            if (user == null)
            {
                user = await _dbContext.Set<User>()
                    .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
            }

            // If user still doesn't exist, create a new one
            if (user == null)
            {
                // Create a random password for the user (they'll never use it)
                string salt;
                var randomPassword = Guid.NewGuid().ToString();
                var passwordHash = _authLogic.HashPassword(randomPassword, out salt);

                user = new User
                {
                    Email = email,
                    Name = name,
                    TenantId = tenantId,
                    Role = userType,
                    IsVerified = true, // Google users are already verified
                    NotificationsEnabled = true,
                    LastLoginDate = DateTime.UtcNow,
                    PointBalance = 0,
                    Password = passwordHash,
                    PasswordSalt = salt,
                    GoogleId = googleId
                };

                await _dbContext.Set<User>().AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // Update Google ID if it's not set
                if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = googleId;
                    user.IsVerified = true; // Mark as verified since they've authenticated with Google
                }

                // Update last login date
                user.LastLoginDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }

            // Verify expiration of points for end users
            if (user.Role == UserType.EndUser && _loyaltyService != null)
            {
                try
                {
                    await _loyaltyService.CheckPointsExpirationAsync(user.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking point expiration: {ex.Message}");
                }
            }

            // Actualizar la fecha del último login (siempre)
            user.LastLoginDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            // Generate token
            return await GenerateJwtToken(
                user.Id,
                user.Email,
                user.Name,
                user.Role,
                user.TenantId ?? null,
                user.BranchId ?? null,
                user.GoogleId
            );
        }

        /// <summary>
        /// Registra a un usuario
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Usuario registrado</returns>
        public async Task<UserSessionDTO> Signup(string email, string password, string name, int tenantId, HttpContext httpContext)
        {
            string salt;
            var passwordHash = _authLogic.HashPassword(password, out salt);

            // Obtener userType del contexto HTTP (para signup siempre será EndUser, pero lo verificamos)
            UserType userType = (UserType)(httpContext.Items["UserType"] ?? UserType.EndUser);

            // Verificar si ya existe un usuario con el mismo email, tenantId y rol
            var existingUser = await _userRepository.GetQueryable()
                .Where(u => u.Email == email && u.TenantId == tenantId && u.Role == userType)
                .FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception($"Ya existe un usuario con el email {email} para el tenant {tenantId} con el rol {userType}");
            }

            var existingTenant = await _tenantRepository.GetQueryable().FirstOrDefaultAsync(t => t.Id == tenantId);
            if (existingTenant == null)
            {
                throw new Exception($"El tenant {tenantId} no existe");
            }

            var newUser = new User
            {
                Email = email,
                Name = name,
                TenantId = tenantId,
                Role = userType,
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
                null,
                null // GoogleId is null for regular signup
            );
        }
    }
}
