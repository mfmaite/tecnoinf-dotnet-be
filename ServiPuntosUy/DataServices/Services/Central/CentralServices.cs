using ServiPuntosUy.DTO;
using System.Linq.Expressions;
using ServiPuntosUy.Models.DAO;
using System.Text.RegularExpressions;
using ServiPuntosUy.DAO.Data.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.CommonLogic;

namespace ServiPuntosUy.DataServices.Services.Central
{
    // Implementación del servicio de tenant para el administrador central
    public class TenantService : ICentralTenantService
    {
        private readonly IGenericRepository<DAO.Models.Central.Tenant> _tenantRepository;
        private readonly IGenericRepository<DAO.Models.Central.LoyaltyConfig> _loyaltyConfigRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IAuthLogic _authLogic;

        public TenantService(
            IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
            IGenericRepository<User> userRepository,
            IAuthLogic authLogic,
            IGenericRepository<DAO.Models.Central.LoyaltyConfig> loyaltyConfigRepository)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _authLogic = authLogic;
            _loyaltyConfigRepository = loyaltyConfigRepository;
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

        public TenantDTO CreateTenant(string tenantName, string pointsName, int pointsValue, decimal accumulationRule, int expiricyPolicyDays) {
            // Validar que tenantId contenga solo letras A-Z (mayus y minus)
            if (!Regex.IsMatch(tenantName, @"^[a-zA-Z]+$"))
            {
                throw new ArgumentException("El tenant name solo puede contener letras mayúsculas de la A a la Z.");
            }

            Console.WriteLine($"Creating tenant: {tenantName}");

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

            // Crear la configuración de lealtad para el tenant
            var newLoyaltyConfig = new DAO.Models.Central.LoyaltyConfig {
                TenantId = createdTenant.Id,
                PointsName = pointsName,
                PointsValue = pointsValue,
                AccumulationRule = accumulationRule,
                ExpiricyPolicyDays = expiricyPolicyDays
            };

            var createdLoyaltyConfig = _loyaltyConfigRepository.AddAsync(newLoyaltyConfig).GetAwaiter().GetResult();
            _loyaltyConfigRepository.SaveChangesAsync().GetAwaiter().GetResult();

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
        private readonly CentralDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<DAO.Models.Central.LoyaltyConfig> _loyaltyConfigRepository;

        public LoyaltyService(CentralDbContext dbContext, IConfiguration configuration, IGenericRepository<DAO.Models.Central.LoyaltyConfig> loyaltyConfigRepository)
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
        public Task<LoyaltyConfigDTO> GetLoyaltyConfigAsync(string tenantId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Actualiza la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="config">Configuración de lealtad</param>
        /// <returns>Configuración de lealtad actualizada</returns>
        public Task<LoyaltyConfigDTO> UpdateLoyaltyConfigAsync(LoyaltyConfigDTO config)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene el saldo de puntos de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Saldo de puntos</returns>
        public Task<int> GetPointsBalanceAsync(int userId, string tenantId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registra una transacción de lealtad
        /// </summary>
        /// <param name="transaction">Datos de la transacción</param>
        /// <returns>Transacción registrada</returns>
        public Task<LoyaltyTransactionDTO> RegisterTransactionAsync(LoyaltyTransactionDTO transaction)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene el historial de transacciones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Historial de transacciones</returns>
        public Task<IEnumerable<LoyaltyTransactionDTO>> GetTransactionHistoryAsync(int userId, string tenantId, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Canjea puntos por un producto o servicio
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="points">Puntos a canjear</param>
        /// <param name="productId">ID del producto (opcional)</param>
        /// <returns>ID de la redención</returns>
        public Task<int> RedeemPointsAsync(int userId, string tenantId, int points, int? productId = null)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica un código QR de redención
        /// </summary>
        /// <param name="qrCode">Código QR</param>
        /// <param name="branchId">ID de la estación</param>
        /// <returns>ID de la redención</returns>
        public Task<int> VerifyRedemptionQrAsync(string qrCode, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Confirma una redención
        /// </summary>
        /// <param name="redemptionId">ID de la redención</param>
        /// <param name="branchId">ID de la estación</param>
        /// <returns>True si la confirmación es exitosa, false en caso contrario</returns>
        public Task<bool> ConfirmRedemptionAsync(int redemptionId, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calcula los puntos a otorgar por una compra
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="amount">Monto de la compra</param>
        /// <param name="productCategory">Categoría del producto</param>
        /// <returns>Puntos a otorgar</returns>
        public Task<int> CalculatePointsAsync(string tenantId, decimal amount, string productCategory)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene estadísticas de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Estadísticas de lealtad</returns>
        public Task<object> GetLoyaltyStatsAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
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
