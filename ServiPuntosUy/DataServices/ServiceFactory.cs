using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.Models.DAO;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DataServices.Services.EndUser;
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
        }

        private void ConfigureTenantServices(string tenantId)
        {
            // Registrar servicios para el administrador de tenant
            _serviceCollection.AddScoped<ILoyaltyService, Services.Tenant.LoyaltyService>();
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    null, // No necesitamos LoyaltyService para Tenant
                    tenantId));

            _serviceCollection.AddScoped<ITenantBranchService, TenantBranchService>();
            _serviceCollection.AddScoped<IPromotionService, Services.Tenant.PromotionService>();
            _serviceCollection.AddScoped<IProductService, Services.Tenant.ProductService>();
            _serviceCollection.AddScoped<IUserService, Services.Tenant.UserService>();
            _serviceCollection.AddScoped<INotificationService, Services.Tenant.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, Services.Tenant.VerificationService>();
            _serviceCollection.AddScoped<IReportingService, Services.Tenant.ReportingService>();
            _serviceCollection.AddScoped<IPaymentService, Services.Tenant.PaymentService>();
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
                    null, // No necesitamos LoyaltyService para Branch
                    tenantId));

            // Registrar los servicios implementados
            _serviceCollection.AddScoped<IBranchService>(sp =>
                new BranchService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId,
                    branchId));


            _serviceCollection.AddScoped<IPromotionService, Services.Branch.PromotionService>();
            // _serviceCollection.AddScoped<IProductService, Services.Branch.ProductService>();
            _serviceCollection.AddScoped<IUserService, Services.Branch.UserService>();
            _serviceCollection.AddScoped<INotificationService, Services.Branch.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, Services.Branch.VerificationService>();

        }

        private void ConfigureEndUserServices(string tenantId)
        {
            // Registrar servicios para el usuario final
            
            // Primero registramos el LoyaltyService
            _serviceCollection.AddScoped<ILoyaltyService>(sp => 
                new Services.EndUser.LoyaltyService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    tenantId));
            
            // Luego registramos el AuthService con la dependencia de LoyaltyService
            _serviceCollection.AddScoped<IAuthService>(sp =>
                new CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
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
        }
    }
}
