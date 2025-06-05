using ServiPuntosUy.DTO;
using System.Linq.Expressions;
using ServiPuntosUy.Models.DAO;
using System.Text.RegularExpressions;
using ServiPuntosUy.DAO.Data.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using Microsoft.EntityFrameworkCore;

namespace ServiPuntosUy.DataServices.Services.Central
{
    // Implementación del servicio de tenant para el administrador central
    public class TenantService : ICentralTenantService
    {
        private readonly IGenericRepository<DAO.Models.Central.Tenant> _tenantRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IAuthLogic _authLogic;

        public TenantService(
            IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
            IGenericRepository<User> userRepository,
            IAuthLogic authLogic)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _authLogic = authLogic;
        }

        // Métodos del Tenant

        public TenantDTO GetTenantDTO(DAO.Models.Central.Tenant tenant) {
            return new TenantDTO {
                Id = tenant.Id,
                Name = tenant.Name,
            };
        }

        public TenantDTO GetTenantById(int id)
        {
            var condicion = new List<Expression<Func<DAO.Models.Central.Tenant, bool>>>
            {
                tenant => tenant.Id == id
            };

            var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.Id == id);
            return tenant != null ? GetTenantDTO(tenant) : null;
        }

        public TenantDTO GetTenantByTenantName(string tenantName)
        {
            var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.Name == tenantName);
            return tenant != null ? GetTenantDTO(tenant) : null;
        }

        public TenantDTO CreateTenant(string tenantName) {
            // Validar que tenantId contenga solo letras A-Z (mayus y minus)
            if (!Regex.IsMatch(tenantName, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException("El tenant name solo puede contener letras mayúsculas de la A a la Z.");
            }

            // Verificar que el tenantId no exista
            bool tenantExists = _tenantRepository.GetQueryable().Any(t => t.Name == tenantName);
            if (tenantExists)
            {
                throw new ArgumentException($"El Tenant '{tenantName}' ya existe. Debe ser único.");
            }

            // Crear el nuevo tenant
            var newTenant = new DAO.Models.Central.Tenant {
                Name = tenantName,
            };

            var createdTenant = _tenantRepository.AddAsync(newTenant).GetAwaiter().GetResult();
            _tenantRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Crear un usuario administrador para el tenant
            CreateTenantAdminUser(createdTenant);

            return GetTenantDTO(createdTenant);
        }

        /// <summary>
        /// Crea un usuario administrador para un tenant
        /// </summary>
        /// <param name="tenant">Tenant al que pertenecerá el usuario</param>
        private void CreateTenantAdminUser(DAO.Models.Central.Tenant tenant)
        {
            // Generar email y contraseña para el administrador del tenant
            string adminEmail = $"{tenant.Name}-admin@mail.com";
            string adminPassword = $"{tenant.Name}-admin-password";

            // Generar hash y salt para la contraseña
            string passwordHash = _authLogic.HashPassword(adminPassword, out string passwordSalt);

            // Crear el usuario administrador
            var adminUser = new User
            {
                TenantId = tenant.Id,
                Email = adminEmail,
                Name = $"Default {tenant.Name} Administrator",
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserType.Tenant, // Tipo de usuario: Tenant admin
                IsVerified = true, // El usuario ya está verificado
                NotificationsEnabled = true
            };

            // Guardar el usuario en la base de datos
            _userRepository.AddAsync(adminUser).GetAwaiter().GetResult();
            _userRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public TenantDTO[] GetTenantsList() {
            var tenants = _tenantRepository.GetQueryable().ToList();
            return tenants.Select(t => GetTenantDTO(t)).ToArray();
        }

        public void DeleteTenant(int id) {
            // Eliminar todos los usuarios del tenant
            var users = _userRepository.GetQueryable().Where(u => u.TenantId == id).ToList();
            foreach (var user in users) {
                _userRepository.DeleteAsync(user.Id).GetAwaiter().GetResult();
            }
            _userRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Eliminar el tenant
            var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.Id == id);

            if (tenant == null) {
                throw new ArgumentException("No se pudo eliminar el tenant");
            }

            _tenantRepository.DeleteAsync(tenant.Id).GetAwaiter().GetResult();
            _tenantRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public TenantDTO UpdateTenant(int id, string newName) {
            // Validar que tenantId contenga solo letras A-Z (mayus y minus)
            if (!Regex.IsMatch(newName, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException("El tenant name solo puede contener letras mayúsculas de la A a la Z.");
            }

            var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.Id == id);

            if (tenant == null) {
                throw new ArgumentException($"No existe un tenant con el ID {id}");
            }

            tenant.Name = newName;
            _tenantRepository.UpdateAsync(tenant).GetAwaiter().GetResult();
            _tenantRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return GetTenantDTO(tenant);
        }
    }

    /// <summary>
    /// Implementación del servicio de lealtad para el administrador central
    /// </summary>
    public class LoyaltyService : ILoyaltyService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<DAO.Models.Central.LoyaltyConfig> _loyaltyConfigRepository;

        public LoyaltyService(DbContext dbContext, IConfiguration configuration, IGenericRepository<DAO.Models.Central.LoyaltyConfig> loyaltyConfigRepository)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _loyaltyConfigRepository = loyaltyConfigRepository;
        }

        /// <summary>
        /// Convierte un modelo de configuración de lealtad a DTO
        /// </summary>
        /// <param name="loyaltyConfig">Modelo de configuración de lealtad</param>
        /// <returns>DTO de configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyConfigDTO(DAO.Models.Central.LoyaltyConfig loyaltyConfig) {
            return new LoyaltyConfigDTO {
                Id = loyaltyConfig.Id,
                TenantId = loyaltyConfig.TenantId,
                PointsName = loyaltyConfig.PointsName,
                PointsValue = loyaltyConfig.PointsValue,
                AccumulationRule = loyaltyConfig.AccumulationRule,
                ExpiricyPolicyDays = loyaltyConfig.ExpiricyPolicyDays
            };
        }

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyProgram(int tenantId)
        {
            var loyaltyConfig = _loyaltyConfigRepository.GetQueryable().FirstOrDefault(lc => lc.TenantId == tenantId);

            if (loyaltyConfig == null) {
                throw new ArgumentException($"No existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            return GetLoyaltyConfigDTO(loyaltyConfig);
        }

        /// <summary>
        /// Verifica si los puntos del usuario han expirado según la política de expiración
        /// y actualiza la fecha del último login
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si los puntos expiraron, False en caso contrario</returns>
        public Task<bool> CheckPointsExpirationAsync(int userId)
        {
            // Para usuarios Central, no aplicamos la lógica de expiración de puntos
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Implementación del servicio de promociones para el administrador central
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PromotionService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz IPromotionService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de productos para el administrador central
    /// </summary>



    /// <summary>
    /// Implementación del servicio de usuarios para el administrador central
    /// </summary>
    public class UserService : IUserService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz IUserService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de notificaciones para el administrador central
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public NotificationService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz INotificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de verificación para el administrador central
    /// </summary>
    public class VerificationService : IVerificationService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public VerificationService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz IVerificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de reportes para el administrador central
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public ReportingService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz IReportingService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de pagos para el administrador central
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PaymentService(CentralDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Implementar los métodos de la interfaz IPaymentService
        // Esta es una implementación básica para el scaffold
    }
}
