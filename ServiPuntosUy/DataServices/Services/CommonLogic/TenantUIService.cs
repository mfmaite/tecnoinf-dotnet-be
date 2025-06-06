using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using ServiPuntosUy.Models.DAO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación del servicio de gestión de UI de tenants
    /// </summary>
    public class TenantUIService : ITenantUIService
    {
        private readonly IGenericRepository<TenantUI> _tenantUIRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public TenantUIService(
            IGenericRepository<TenantUI> tenantUIRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantUIRepository = tenantUIRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        
        /// <summary>
        /// Convierte un modelo de TenantUI a DTO
        /// </summary>
        /// <param name="tenantUI">Modelo de TenantUI</param>
        /// <returns>DTO de TenantUI</returns>
        private TenantUIDTO GetTenantUIDTO(TenantUI tenantUI)
        {
            return new TenantUIDTO
            {
                Id = tenantUI.Id,
                TenantId = tenantUI.TenantId,
                LogoUrl = tenantUI.LogoUrl,
                PrimaryColor = tenantUI.PrimaryColor,
                SecondaryColor = tenantUI.SecondaryColor
            };
        }
        
        /// <inheritdoc/>
        public async Task<TenantUIDTO> GetTenantUIAsync(int tenantId)
        {
            var tenantUI = _tenantUIRepository.GetQueryable()
                .FirstOrDefault(t => t.TenantId == tenantId);
                
            if (tenantUI == null)
            {
                throw new ArgumentException($"No existe configuración UI para el tenant con ID {tenantId}");
            }
            
            return GetTenantUIDTO(tenantUI);
        }
        
        /// <inheritdoc/>
        public async Task<TenantUIDTO> UpdateTenantUIAsync(int tenantId, string logoUrl, string primaryColor, string secondaryColor, HttpContext httpContext)
        {
            // Verificar si el usuario es tenantAdmin y tiene acceso al tenant
            var userType = (UserType)httpContext.Items["UserType"];
            var currentTenantIdStr = httpContext.Items["CurrentTenant"] as string;
            
            if (userType != UserType.Tenant || !currentTenantIdStr.Equals(tenantId.ToString()))
            {
                throw new UnauthorizedAccessException("Solo los administradores del tenant pueden modificar la UI");
            }
            
            var tenantUI = _tenantUIRepository.GetQueryable()
                .FirstOrDefault(t => t.TenantId == tenantId);
                
            if (tenantUI == null)
            {
                throw new ArgumentException($"No existe configuración UI para el tenant con ID {tenantId}");
            }
            
            // Actualizar los valores
            tenantUI.LogoUrl = logoUrl;
            tenantUI.PrimaryColor = primaryColor;
            tenantUI.SecondaryColor = secondaryColor;
            
            await _tenantUIRepository.UpdateAsync(tenantUI);
            await _tenantUIRepository.SaveChangesAsync();
            
            return GetTenantUIDTO(tenantUI);
        }
    }
}
