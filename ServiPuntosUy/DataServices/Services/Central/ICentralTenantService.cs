using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DataServices.Services.Central
{
    /// <summary>
    /// Interfaz para el servicio de gestión de tenants (exclusivo para el administrador central)
    /// </summary>
    public interface ICentralTenantService
    {
        /// <summary>
        /// Convierte un modelo de tenant a DTO
        /// </summary>
        /// <param name="tenant">Modelo de tenant</param>
        /// <returns>DTO de tenant</returns>
        TenantDTO GetTenantDTO(DAO.Models.Central.Tenant tenant);
        
        /// <summary>
        /// Obtiene un tenant por su ID
        /// </summary>
        /// <param name="id">ID del tenant</param>
        /// <returns>DTO de tenant</returns>
        TenantDTO GetTenantById(int id);
        
        /// <summary>
        /// Obtiene un tenant por su TenantId
        /// </summary>
        /// <param name="tenantId">TenantId del tenant</param>
        /// <returns>DTO de tenant</returns>
        TenantDTO GetTenantByTenantId(string tenantId);
        
        /// <summary>
        /// Crea un nuevo tenant
        /// </summary>
        /// <param name="tenantId">TenantId del tenant</param>
        /// <returns>DTO del tenant creado</returns>
        TenantDTO CreateTenant(string tenantId);
    }
}
