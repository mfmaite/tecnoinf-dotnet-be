using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para el servicio de gestión de UI de tenants
    /// </summary>
    public interface ITenantUIService
    {
        /// <summary>
        /// Obtiene la UI de un tenant por su ID
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>DTO de la UI del tenant</returns>
        Task<TenantUIDTO> GetTenantUIAsync(int tenantId);
        
        /// <summary>
        /// Actualiza la UI de un tenant (solo para tenantAdmin)
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="logoUrl">URL del logo</param>
        /// <param name="primaryColor">Color primario</param>
        /// <param name="secondaryColor">Color secundario</param>
        /// <param name="httpContext">Contexto HTTP para verificar permisos</param>
        /// <returns>DTO de la UI del tenant actualizada</returns>
        Task<TenantUIDTO> UpdateTenantUIAsync(int tenantId, string logoUrl, string primaryColor, string secondaryColor, HttpContext httpContext);
    }
}
