using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.Models.DAO;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DataServices.Services.EndUser;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DAO.Data.Central;
namespace ServiPuntosUy.DataServices
{
    /// <summary>
    /// Implementación de la fábrica de servicios que proporciona acceso a los servicios según el tenant y tipo de usuario
    /// </summary>
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _scopedServiceProvider;

        public ServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _serviceCollection = new ServiceCollection();
            _scopedServiceProvider = null;
        }

        public T GetService<T>() where T : class
        {
            // Si tenemos un proveedor de servicios configurado para el tenant/usuario actual, usarlo
            if (_scopedServiceProvider != null)
            {
                return _scopedServiceProvider.GetService<T>();
            }

            // Si no, usar el proveedor de servicios global
            return _serviceProvider.GetService<T>();
        }

        public void ConfigureServices(string tenantId, UserType userType, HttpContext httpContext = null)
        {
            // Verificar si el usuario está autenticado
            bool isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated == true;

            // Si no está autenticado, solo configurar servicios comunes para login
            if (!isAuthenticated)
            {
                ConfigureCommonServicesForLogin();
                return;
            }

            // El resto del código original para usuarios autenticados
            // Limpiar la colección de servicios antes de configurar
            _serviceCollection.Clear();

            // Configurar servicios comunes para todos los tipos de usuario
            ConfigureCommonServices(tenantId);

            // Registrar servicios específicos según el tipo de usuario
            switch (userType)
            {
                case UserType.Central:
                    ConfigureCentralServices();
                    break;
                case UserType.Tenant:
                    ConfigureTenantServices(tenantId);
                    break;
                case UserType.Branch:
                    // Obtener el branchId del contexto HTTP si está disponible
                    int branchId = 0;
                    if (httpContext != null && httpContext.Items.ContainsKey("BranchId"))
                    {
                        branchId = (int)httpContext.Items["BranchId"];
                    }
                    ConfigureBranchServices(tenantId, branchId);
                    break;
                case UserType.EndUser:
                    ConfigureEndUserServices(tenantId);
                    break;
                default:
                    throw new ArgumentException($"Tipo de usuario no soportado: {userType}");
            }

            // Construir el proveedor de servicios para este tenant/usuario
            _scopedServiceProvider = _serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Configura solo los servicios comunes necesarios para el login
        /// </summary>
        private void ConfigureCommonServicesForLogin()
        {
            _serviceCollection.Clear();

            // Registrar servicios mínimos necesarios para login
            _serviceCollection.AddSingleton(_configuration);

            // Obtener servicios globales ya registrados en Program.cs
            _serviceCollection.AddScoped<IAuthLogic>(sp => _serviceProvider.GetRequiredService<IAuthLogic>());
            _serviceCollection.AddScoped<ITenantResolver>(sp => _serviceProvider.GetRequiredService<ITenantResolver>());

            // Obtener el DbContext del contenedor principal
            _serviceCollection.AddScoped<DbContext>(sp => _serviceProvider.GetRequiredService<DbContext>());

            // Registrar el GenericRepository que usa el DbContext
            _serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registrar el servicio público de TenantUI (disponible sin autenticación)
            _serviceCollection.AddScoped<IPublicTenantUIService>(sp => 
                new PublicTenantUIService(
                    sp.GetRequiredService<IGenericRepository<TenantUI>>(),
                    sp.GetRequiredService<ITenantResolver>()));

            // Registrar un servicio de autenticación básico
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.User>>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.Tenant>>(),
                    null, // No necesitamos LoyaltyService para login
                    null)); // No hay tenant para login

            // Construir el proveedor de servicios para login
            _scopedServiceProvider = _serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Configura los servicios básicos comunes a todos los tipos de usuario
        /// </summary>
        private void ConfigureCommonServices(string tenantId)
        {
            // Registrar servicios comunes
            _serviceCollection.AddSingleton(_configuration);

            // Obtener servicios globales ya registrados en Program.cs
            _serviceCollection.AddScoped<IAuthLogic>(sp => _serviceProvider.GetRequiredService<IAuthLogic>());
            _serviceCollection.AddScoped<ITenantResolver>(sp => _serviceProvider.GetRequiredService<ITenantResolver>());

            // Registrar el TenantAccessor para proporcionar el tenant actual a los servicios
            _serviceCollection.AddScoped<ITenantAccessor>(sp => new TenantAccessor(tenantId));

            // Obtener el DbContext del contenedor principal
            _serviceCollection.AddScoped<DbContext>(sp => _serviceProvider.GetRequiredService<DbContext>());

            // Registrar el GenericRepository que usa el DbContext
            _serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registrar el servicio de TenantUI
            _serviceCollection.AddScoped<ITenantUIService, TenantUIService>();

            // Registrar HttpContextAccessor si no está registrado
            _serviceCollection.AddHttpContextAccessor();
        }

        private void ConfigureCentralServices()
        {
            // Registrar servicios para el administrador central
            _serviceCollection.AddScoped<ICentralTenantService, TenantService>();
            _serviceCollection.AddScoped<ILoyaltyService, Services.Central.LoyaltyService>();
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.User>>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.Tenant>>(),
                    null, // No necesitamos LoyaltyService para Central
                    null)); // null para Central

            // Registrar los demás servicios para el administrador central
            _serviceCollection.AddScoped<IPromotionService, Services.Central.PromotionService>();
            // _serviceCollection.AddScoped<IProductService, Services.Central.ProductService>();
            _serviceCollection.AddScoped<IUserService, Services.Central.UserService>();
            _serviceCollection.AddScoped<INotificationService, Services.Central.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, Services.Central.VerificationService>();
            _serviceCollection.AddScoped<IReportingService, Services.Central.ReportingService>();
            _serviceCollection.AddScoped<IPaymentService, Services.Central.PaymentService>();
            _serviceCollection.AddScoped<IStatisticsService>(sp =>
                new Services.Central.StatisticsService(
                    _serviceProvider.GetRequiredService<CentralDbContext>(),
                    _configuration));
        }

        private void ConfigureTenantServices(string tenantId)
        {
            // Registrar servicios para el administrador de tenant
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.User>>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.Tenant>>(),
                    null, // No necesitamos LoyaltyService para Tenant
                    tenantId));

            _serviceCollection.AddScoped<ILoyaltyService>(sp =>
                new Services.Tenant.LoyaltyService(
                    sp.GetRequiredService<DbContext>(),
                    sp.GetRequiredService<IGenericRepository<LoyaltyConfig>>()));

            _serviceCollection.AddScoped<ITenantBranchService, Services.Tenant.TenantBranchService>();
            _serviceCollection.AddScoped<IBranchService, BranchService>();
            _serviceCollection.AddScoped<IPromotionService, Services.Tenant.PromotionService>();
            _serviceCollection.AddScoped<IProductService, Services.Tenant.ProductService>();
            _serviceCollection.AddScoped<IUserService, Services.Tenant.UserService>();
            _serviceCollection.AddScoped<INotificationService, Services.Tenant.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, Services.Tenant.VerificationService>();
            _serviceCollection.AddScoped<IReportingService, Services.Tenant.ReportingService>();
            _serviceCollection.AddScoped<IPaymentService, Services.Tenant.PaymentService>();
            _serviceCollection.AddScoped<IStatisticsService>(sp =>
                new Services.Tenant.StatisticsService(
                    _serviceProvider.GetRequiredService<CentralDbContext>(),
                    _configuration,
                    sp.GetRequiredService<ITenantAccessor>()));
        }

        private void ConfigureBranchServices(string tenantId, int branchId)
        {
            // Registrar servicios para el administrador de branch
            _serviceCollection.AddScoped<ILoyaltyService, Services.Branch.LoyaltyService>();
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.User>>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.Tenant>>(),
                    null, // No necesitamos LoyaltyService para Branch
                    tenantId));

            // Registrar los servicios implementados
            // _serviceCollection.AddScoped<IBranchService>(sp => _serviceProvider.GetRequiredService<IBranchService>());
            _serviceCollection.AddScoped<IBranchService>(sp =>
                new BranchService(
                    sp.GetRequiredService<IGenericRepository<ServiPuntosUy.DAO.Models.Central.Branch>>(),
                    sp.GetRequiredService<IGenericRepository<ServiPuntosUy.DAO.Models.Central.ProductStock>>(),
                    sp.GetRequiredService<IGenericRepository<ServiPuntosUy.DAO.Models.Central.Product>>()));


            _serviceCollection.AddScoped<IPromotionService>(sp =>
                new Services.Branch.PromotionService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId,
                    branchId));
            // _serviceCollection.AddScoped<IProductService, Services.Branch.ProductService>();
            _serviceCollection.AddScoped<IUserService>(sp =>
                new Services.Branch.UserService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId,
                    branchId));
            _serviceCollection.AddScoped<INotificationService>(sp =>
                new Services.Branch.NotificationService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId,
                    branchId));
            _serviceCollection.AddScoped<IVerificationService>(sp =>
                new Services.Branch.VerificationService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId,
                    branchId));
            _serviceCollection.AddScoped<IFuelService>(sp =>
                new Services.Branch.FuelService(
                    sp.GetRequiredService<IGenericRepository<FuelPrices>>(),
                    branchId));
            _serviceCollection.AddScoped<IStatisticsService>(sp =>
                new Services.Branch.StatisticsService(
                    _serviceProvider.GetRequiredService<CentralDbContext>(),
                    _configuration,
                    sp.GetRequiredService<ITenantAccessor>(),
                    sp.GetRequiredService<IHttpContextAccessor>()));

        }

        private void ConfigureEndUserServices(string tenantId)
        {
            // Registrar servicios para el usuario final

            // Primero registramos el LoyaltyService
            _serviceCollection.AddScoped<ILoyaltyService>(sp =>
                new Services.EndUser.LoyaltyService(
                    sp.GetRequiredService<DbContext>(),
                    sp.GetRequiredService<IGenericRepository<LoyaltyConfig>>()));

            // Luego registramos el AuthService con la dependencia de LoyaltyService
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.User>>(),
                    sp.GetRequiredService<IGenericRepository<DAO.Models.Central.Tenant>>(),
                    sp.GetRequiredService<ILoyaltyService>(), // Inyectamos el servicio de lealtad
                    tenantId));

            // Registrar los demás servicios
            _serviceCollection.AddScoped<IPromotionService, Services.EndUser.PromotionService>();
            _serviceCollection.AddScoped<IProductService, Services.EndUser.ProductService>();
            _serviceCollection.AddScoped<IUserService, Services.EndUser.UserService>();
            _serviceCollection.AddScoped<INotificationService, Services.EndUser.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, Services.EndUser.VerificationService>();
            _serviceCollection.AddScoped<IPaymentService, Services.EndUser.PaymentService>();
            _serviceCollection.AddScoped<IVEAIService, Services.EndUser.VEAIService>();
            _serviceCollection.AddScoped<IFuelService, Services.EndUser.FuelService>();
            _serviceCollection.AddScoped<ITenantBranchService, Services.EndUser.TenantBranchService>();
        }
    }
}
