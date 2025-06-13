using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Models.DAO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación del servicio público de UI de tenants (sin autenticación)
    /// </summary>
    public class PublicTenantUIService : IPublicTenantUIService
    {
        private readonly IGenericRepository<TenantUI> _tenantUIRepository;
        private readonly ITenantResolver _tenantResolver;
        
        public PublicTenantUIService(
            IGenericRepository<TenantUI> tenantUIRepository,
            ITenantResolver tenantResolver)
        {
            _tenantUIRepository = tenantUIRepository;
            _tenantResolver = tenantResolver;
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
        public async Task<TenantUIDTO> GetTenantUIAsync(HttpContext httpContext)
        {
            // Obtener el tenantId del contexto HTTP
            string tenantIdStr = httpContext.Items["CurrentTenant"] as string;
            
            if (string.IsNullOrEmpty(tenantIdStr))
            {
                // Si no está en el contexto, intentar resolverlo
                tenantIdStr = _tenantResolver.ResolveTenantId(httpContext);
                
                if (string.IsNullOrEmpty(tenantIdStr))
                {
                    throw new ArgumentException("No se pudo determinar el tenant actual");
                }
            }
            
            if (!int.TryParse(tenantIdStr, out int tenantId))
            {
                throw new ArgumentException($"ID de tenant inválido: {tenantIdStr}");
            }
            
            var tenantUI = _tenantUIRepository.GetQueryable()
                .FirstOrDefault(t => t.TenantId == tenantId);
                
            if (tenantUI == null)
            {
                throw new ArgumentException($"No existe configuración UI para el tenant con ID {tenantId}");
            }
            
            return GetTenantUIDTO(tenantUI);
        }
    }
}
