using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DataServices.Services.CommonLogic;

namespace ServiPuntosUy.Controllers.Base
{
    /// <summary>
    /// Controlador base que proporciona funcionalidad común para todos los controladores
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly IServiceFactory _serviceFactory;

        public BaseController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        /// <summary>
        /// Obtiene el tenant actual del contexto HTTP
        /// </summary>
        protected string CurrentTenant => HttpContext.Items["CurrentTenant"] as string;

        /// <summary>
        /// Obtiene el tipo de usuario actual del contexto HTTP
        /// </summary>
        protected UserType UserType => HttpContext.Items["UserType"] is UserType userType ? userType : UserType.EndUser;

        /// <summary>
        /// Obtiene el ID de la estación actual del contexto HTTP (solo para administradores de estación)
        /// </summary>
        protected int? BranchId => HttpContext.Items["BranchId"] as int?;

        /// <summary>
        /// Obtiene el usuario actual del token JWT
        /// </summary>
        /// <returns>Información del usuario</returns>
        protected UserDTO ObtainUserFromToken()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());

                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
                var email = jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
                var name = jwtToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
                var tenantId = jwtToken.Claims.FirstOrDefault(x => x.Type == "tenantId")?.Value;
                var userType = jwtToken.Claims.FirstOrDefault(x => x.Type == "userType")?.Value;

                return new UserDTO
                {
                    Id = int.Parse(userId),
                    Name = name,
                    Email = email,
                    TenantId = tenantId,
                    UserType = (UserType)int.Parse(userType)
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el tenant actual del token JWT
        /// </summary>
        /// <returns>ID del tenant</returns>
        protected string ObtainTenantFromToken()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var currentTenant = jwtToken.Claims.FirstOrDefault(x => x.Type == "currentTenant")?.Value;

                return currentTenant;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene el tipo de usuario del token JWT
        /// </summary>
        /// <returns>Tipo de usuario</returns>
        protected UserType ObtainUserTypeFromToken()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return UserType.EndUser;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var userTypeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userType")?.Value;

                if (userTypeClaim != null && Enum.TryParse<UserType>(userTypeClaim, out var userType))
                {
                    return userType;
                }

                return UserType.EndUser;
            }
            catch
            {
                return UserType.EndUser;
            }
        }

        /// <summary>
        /// Obtiene el ID de la estación del token JWT (solo para administradores de estación)
        /// </summary>
        /// <returns>ID de la estación, o null si no aplica</returns>
        protected int? ObtainBranchIdFromToken()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var branchIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "branchId")?.Value;

                if (branchIdClaim != null && int.TryParse(branchIdClaim, out int branchId))
                {
                    return branchId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Verifica si el usuario actual tiene acceso al tenant especificado
        /// </summary>
        /// <param name="tenantId">ID del tenant a verificar</param>
        /// <returns>true si tiene acceso, false en caso contrario</returns>
        protected bool HasAccessToTenant(string tenantId)
        {
            // Si es administrador central, tiene acceso a todos los tenants
            if (UserType == UserType.Central)
            {
                return true;
            }

            // Si es administrador de tenant o estación, solo tiene acceso a su propio tenant
            if (UserType == UserType.Tenant || UserType == UserType.Branch)
            {
                return CurrentTenant == tenantId;
            }

            // Si es usuario final, depende de la lógica de negocio
            // Por ejemplo, un usuario final podría tener acceso a múltiples tenants
            return CurrentTenant == tenantId;
        }

        #region Métodos de ayuda para obtener servicios

        /// <summary>
        /// Obtiene el servicio de autenticación
        /// </summary>
        protected IAuthService AuthService => _serviceFactory.GetService<IAuthService>();

        /// <summary>
        /// Obtiene el servicio de tenant
        /// </summary>
        protected ICentralTenantService TenantService => _serviceFactory.GetService<ICentralTenantService>();

        /// <summary>
        /// Obtiene el servicio de estaciones
        /// </summary>
        protected IBranchService BranchService => _serviceFactory.GetService<IBranchService>();

        /// <summary>
        /// Obtiene el servicio de fidelización
        /// </summary>
        protected ILoyaltyService LoyaltyService => _serviceFactory.GetService<ILoyaltyService>();

        /// <summary>
        /// Obtiene el servicio de promociones
        /// </summary>
        protected IPromotionService PromotionService => _serviceFactory.GetService<IPromotionService>();

        /// <summary>
        /// Obtiene el servicio de productos
        /// </summary>
        protected IProductService ProductService => _serviceFactory.GetService<IProductService>();

        /// <summary>
        /// Obtiene el servicio de usuarios
        /// </summary>
        protected IUserService UserService => _serviceFactory.GetService<IUserService>();

        /// <summary>
        /// Obtiene el servicio de notificaciones
        /// </summary>
        protected INotificationService NotificationService => _serviceFactory.GetService<INotificationService>();

        /// <summary>
        /// Obtiene el servicio de verificación
        /// </summary>
        protected IVerificationService VerificationService => _serviceFactory.GetService<IVerificationService>();

        /// <summary>
        /// Obtiene el servicio de reportes
        /// </summary>
        protected IReportingService ReportingService => _serviceFactory.GetService<IReportingService>();

        /// <summary>
        /// Obtiene el servicio de pagos
        /// </summary>
        protected IPaymentService PaymentService => _serviceFactory.GetService<IPaymentService>();

        /// <summary>
        /// Obtiene el servicio de branches para tenant
        /// </summary>
        protected ITenantBranchService TenantBranchService => _serviceFactory.GetService<ITenantBranchService>();

        /// <summary>
        /// Obtiene el servicio de VEAI
        /// </summary>
        protected IVEAIService VEAIService => _serviceFactory.GetService<IVEAIService>();

        /// <summary>
        /// Obtiene el servicio de precios de combustible
        /// </summary>
        protected IFuelService FuelService => _serviceFactory.GetService<IFuelService>();

        /// <summary>
        /// Obtiene el servicio de estadísticas
        /// </summary>
        protected IStatisticsService StatisticsService => _serviceFactory.GetService<IStatisticsService>();

        /// <summary>
        /// Obtiene el servicio público de UI de tenants (sin autenticación)
        /// </summary>
        protected IPublicTenantUIService PublicTenantUIService => _serviceFactory.GetService<IPublicTenantUIService>();

        /// <summary>
        /// Obtiene el servicio de transacciones
        /// </summary>
        protected ITransactionService TransactionService => _serviceFactory.GetService<ITransactionService>();

        /// <summary>
        /// Obtiene el servicio de canjes
        /// </summary>
        protected IRedemptionService RedemptionService => _serviceFactory.GetService<IRedemptionService>();

        /// <summary>
        /// Obtiene el servicio de parámetros generales
        /// </summary>
        protected IGeneralParameterService GeneralParameterService => _serviceFactory.GetService<IGeneralParameterService>();

        /// <summary>
        /// Obtiene el servicio público de tenants (sin autenticación)
        /// </summary>
        protected IPublicTenantService PublicTenantService => _serviceFactory.GetService<IPublicTenantService>();

        #endregion
    }
}
