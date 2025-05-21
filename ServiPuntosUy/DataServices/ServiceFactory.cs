using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiPuntosUy.Models.DAO;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Enums;

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
            
            // Registrar servicios comunes
            _serviceCollection.AddSingleton(_configuration);
            _serviceCollection.AddScoped<IAuthLogic, AuthLogic>();
            _serviceCollection.AddScoped<ITenantResolver, TenantResolver>();
            
            // Registrar el TenantAccessor para proporcionar el tenant actual a los servicios
            _serviceCollection.AddScoped<ITenantAccessor>(sp => new TenantAccessor(tenantId));

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

        private void ConfigureCentralServices()
        {
            // Registrar el DbContext para el administrador central
            _serviceCollection.AddDbContext<ServiPuntosUy.DAO.Data.Central.CentralDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("CentralConnection")));
            
            // Registrar el DbContext como base para el GenericRepository
            _serviceCollection.AddScoped<DbContext>(sp => sp.GetService<ServiPuntosUy.DAO.Data.Central.CentralDbContext>());
            
            // Registrar el GenericRepository
            _serviceCollection.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            
            // Registrar servicios para el administrador central
            _serviceCollection.AddScoped<ICentralTenantService, ServiPuntosUy.DataServices.Services.Central.TenantService>();
            _serviceCollection.AddScoped<IAuthService>(sp => 
                new ServiPuntosUy.DataServices.Services.CommonLogic.CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    null)); // null para Central
            
            // Registrar los demás servicios para el administrador central
            _serviceCollection.AddScoped<ILoyaltyService, ServiPuntosUy.DataServices.Services.Central.LoyaltyService>();
            _serviceCollection.AddScoped<IPromotionService, ServiPuntosUy.DataServices.Services.Central.PromotionService>();
            _serviceCollection.AddScoped<IProductService, ServiPuntosUy.DataServices.Services.Central.ProductService>();
            _serviceCollection.AddScoped<IUserService, ServiPuntosUy.DataServices.Services.Central.UserService>();
            _serviceCollection.AddScoped<INotificationService, ServiPuntosUy.DataServices.Services.Central.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, ServiPuntosUy.DataServices.Services.Central.VerificationService>();
            _serviceCollection.AddScoped<IReportingService, ServiPuntosUy.DataServices.Services.Central.ReportingService>();
            _serviceCollection.AddScoped<IPaymentService, ServiPuntosUy.DataServices.Services.Central.PaymentService>();
        }

        private void ConfigureTenantServices(string tenantId)
        {
            // Registrar servicios para el administrador de tenant
            _serviceCollection.AddScoped<IAuthService>(sp => 
                new ServiPuntosUy.DataServices.Services.CommonLogic.CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    tenantId));
                    
            
            _serviceCollection.AddScoped<ILoyaltyService, ServiPuntosUy.DataServices.Services.Tenant.LoyaltyService>();
            _serviceCollection.AddScoped<IPromotionService, ServiPuntosUy.DataServices.Services.Tenant.PromotionService>();
            _serviceCollection.AddScoped<IProductService, ServiPuntosUy.DataServices.Services.Tenant.ProductService>();
            _serviceCollection.AddScoped<IUserService, ServiPuntosUy.DataServices.Services.Tenant.UserService>();
            _serviceCollection.AddScoped<INotificationService, ServiPuntosUy.DataServices.Services.Tenant.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, ServiPuntosUy.DataServices.Services.Tenant.VerificationService>();
            _serviceCollection.AddScoped<IReportingService, ServiPuntosUy.DataServices.Services.Tenant.ReportingService>();
            _serviceCollection.AddScoped<IPaymentService, ServiPuntosUy.DataServices.Services.Tenant.PaymentService>();
            
        }

        private void ConfigureBranchServices(string tenantId, int branchId)
        {
            // Registrar servicios para el administrador de estación
            _serviceCollection.AddScoped<IAuthService>(sp => 
                new ServiPuntosUy.DataServices.Services.CommonLogic.CommonAuthService(
                    sp.GetRequiredService<DbContext>(),
                    _configuration,
                    sp.GetRequiredService<IAuthLogic>(),
                    tenantId));
            
            // Registrar los servicios implementados
            _serviceCollection.AddScoped<ServiPuntosUy.DataServices.Services.Branch.IBranchService>(sp => 
                new ServiPuntosUy.DataServices.Services.Branch.BranchService(
                    sp.GetRequiredService<DbContext>(), 
                    _configuration, 
                    tenantId, 
                    branchId));
            
            
            _serviceCollection.AddScoped<ILoyaltyService, ServiPuntosUy.DataServices.Services.Branch.LoyaltyService>();
            _serviceCollection.AddScoped<IPromotionService, ServiPuntosUy.DataServices.Services.Branch.PromotionService>();
            _serviceCollection.AddScoped<IProductService, ServiPuntosUy.DataServices.Services.Branch.ProductService>();
            _serviceCollection.AddScoped<IUserService, ServiPuntosUy.DataServices.Services.Branch.UserService>();
            _serviceCollection.AddScoped<INotificationService, ServiPuntosUy.DataServices.Services.Branch.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, ServiPuntosUy.DataServices.Services.Branch.VerificationService>();
            
        }

        private void ConfigureEndUserServices(string tenantId)
        {
            // Registrar servicios para el usuario final
            _serviceCollection.AddScoped<IAuthService>(sp => 
                new ServiPuntosUy.DataServices.Services.CommonLogic.CommonAuthService(
                    sp.GetRequiredService<DbContext>(), 
                    _configuration, 
                    sp.GetRequiredService<IAuthLogic>(), 
                    tenantId));
            
            
            _serviceCollection.AddScoped<ILoyaltyService, ServiPuntosUy.DataServices.Services.EndUser.LoyaltyService>();
            _serviceCollection.AddScoped<IPromotionService, ServiPuntosUy.DataServices.Services.EndUser.PromotionService>();
            _serviceCollection.AddScoped<IProductService, ServiPuntosUy.DataServices.Services.EndUser.ProductService>();
            _serviceCollection.AddScoped<IUserService, ServiPuntosUy.DataServices.Services.EndUser.UserService>();
            _serviceCollection.AddScoped<INotificationService, ServiPuntosUy.DataServices.Services.EndUser.NotificationService>();
            _serviceCollection.AddScoped<IVerificationService, ServiPuntosUy.DataServices.Services.EndUser.VerificationService>();
            _serviceCollection.AddScoped<IPaymentService, ServiPuntosUy.DataServices.Services.EndUser.PaymentService>();
            
        }
    }
}
