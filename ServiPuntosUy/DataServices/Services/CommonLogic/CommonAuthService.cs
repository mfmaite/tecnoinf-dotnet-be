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
using System.Security.Cryptography;

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
        private readonly IEmailService _emailService;

        public CommonAuthService(
            DbContext dbContext,
            IConfiguration configuration,
            IAuthLogic authLogic,
            IGenericRepository<DAO.Models.Central.User> userRepository,
            IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
            ILoyaltyService loyaltyService = null,
            string tenantId = null,
            IEmailService emailService = null)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _authLogic = authLogic;
            _loyaltyService = loyaltyService;
            _tenantId = tenantId;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _emailService = emailService;
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
        /// <param name="isGoogleUser">Indica si el usuario se autenticó con Google</param>
        /// <returns>Token JWT</returns>
        public async Task<UserSessionDTO> GenerateJwtToken(int userId, string email, string name, UserType role, int? tenantId, int? branchId, bool isGoogleUser = false)
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

            // Agregar información de autenticación con Google al token
            if (isGoogleUser)
            {
                claims.Add(new Claim("isGoogleUser", "true"));
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
                user.IsGoogleUser
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
                IsGoogleUser = user.IsGoogleUser
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
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        public async Task<UserSessionDTO?> AuthenticateWithGoogleAsync(
            string idToken,
            string email,
            string name,
            HttpContext httpContext)
        {
            // Obtener tenantId y userType del contexto HTTP
            string? tenantIdStr = httpContext.Items["CurrentTenant"] as string;
            if (string.IsNullOrEmpty(tenantIdStr) || !int.TryParse(tenantIdStr, out int tenantId))
            {
                throw new Exception("No se pudo determinar el tenant para el login con Google");
            }

            UserType userType = (UserType)(httpContext.Items["UserType"] ?? UserType.EndUser);

            // Try to find user by email
            var user = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);

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
                    IsGoogleUser = true
                };

                await _dbContext.Set<User>().AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // Mark as Google user if not already
                if (!user.IsGoogleUser)
                {
                    user.IsGoogleUser = true;
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
                user.IsGoogleUser
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
                false // Not a Google user for regular signup
            );
        }

        /// <summary>
        /// Genera un magic link para el login
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Link completo para el magic link</returns>
        public async Task<string> GenerateMagicLinkAsync(string email, HttpContext httpContext)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El email es requerido");
            }

            // Obtener tenantId y userType del contexto HTTP
            string? tenantIdStr = httpContext.Items["CurrentTenant"] as string;
            int? tenantId = !string.IsNullOrEmpty(tenantIdStr) && int.TryParse(tenantIdStr, out int tid) ? tid : null;

            // Buscar usuario por email, tenantId y rol
            var user = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);

            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // Generar un token único
            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes);

            // Crear el token JWT con la información necesaria
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new[]
            {
                new Claim("userEmail", user.Email),
                new Claim("userType", ((int)user.Role).ToString()),
                new Claim("tenantId", user.TenantId.ToString()),
                new Claim("magicLinkToken", token)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15), // El magic link expira en 15 minutos
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(jwtToken);

            string baseUrl;
            if (httpContext.Request.Headers.TryGetValue("X-Tenant-Name", out var tenantName)) // Viene de la app
            {
                baseUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/auth/redirect";
            }
            else
            {
                // Obtener la URL base de la aplicación
                var frontendUrl = _configuration["AppSettings:FrontendUrl"];
                // Obtener el tenant para el subdominio
                var tenant = await _tenantRepository.GetByIdAsync(user.TenantId ?? -1);
                var tenantSubdomain = tenant != null ? $"{tenant.Name.ToLower().Replace(" ", "")}." : "";

                // Construir la URL con el subdominio del tenant
                if (string.IsNullOrEmpty(frontendUrl))
                {
                    baseUrl = "http://localhost:3000";
                }
                else
                {
                    baseUrl = $"http://{tenantSubdomain}{frontendUrl}";
                }
            }

            // Generar el link completo
            var magicLink = $"{baseUrl}/validate-magic-link?token={Uri.EscapeDataString(tokenString)}";

            // Enviar el email con el magic link
            if (_emailService != null)
            {
                var subject = "Tu link de acceso a ServiPuntosUY";
                var body = $@"
                    <html>
                        <body>
                            <h2>Hola {user.Name},</h2>
                            <p>Has solicitado iniciar sesión en ServiPuntosUY. Haz clic en el siguiente enlace para acceder:</p>
                            <p><a href='{magicLink}'>Iniciar sesión en ServiPuntosUY</a></p>
                            <p>Este enlace expirará en 15 minutos por razones de seguridad.</p>
                            <p>Si no solicitaste este enlace, puedes ignorar este mensaje.</p>
                            <br>
                            <p>Saludos,<br>El equipo de ServiPuntosUY</p>
                        </body>
                    </html>";

                var emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
                if (!emailSent)
                {
                    throw new Exception("Error al enviar el email con el magic link");
                }
            }

            return magicLink;
        }

        /// <summary>
        /// Valida un magic link y genera una sesión
        /// </summary>
        /// <param name="magicLinkToken">Token del magic link</param>
        /// <returns>Token de sesión si el magic link es válido</returns>
        public async Task<UserSessionDTO> ValidateMagicLinkAsync(string magicLinkToken)
        {
            try
            {
                // Validar el token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(magicLinkToken, validationParameters, out validatedToken);

                // Extraer la información del token
                var email = principal.FindFirst("userEmail")?.Value;
                var userTypeStr = principal.FindFirst("userType")?.Value;
                var tenantIdStr = principal.FindFirst("tenantId")?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userTypeStr) || string.IsNullOrEmpty(tenantIdStr))
                {
                    throw new Exception("Token inválido");
                }

                var userType = (UserType)int.Parse(userTypeStr);
                var tenantId = int.Parse(tenantIdStr);

                // Buscar el usuario
                var user = await _dbContext.Set<User>()
                    .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId && u.Role == userType);

                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                // Actualizar la fecha del último login
                user.LastLoginDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                // Generar el token de sesión
                return await GenerateJwtToken(
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Role,
                    user.TenantId,
                    user.BranchId,
                    user.IsGoogleUser
                );
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar el magic link: {ex.Message}");
            }
        }
    }
}
