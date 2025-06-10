using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para el servicio público de UI de tenants (sin autenticación)
    /// </summary>
    public interface IPublicTenantUIService
    {
        /// <summary>
        /// Obtiene la UI del tenant actual (acceso público)
        /// </summary>
        /// <param name="httpContext">Contexto HTTP que contiene la información del tenant</param>
        /// <returns>DTO de la UI del tenant</returns>
        Task<TenantUIDTO> GetTenantUIAsync(HttpContext httpContext);
    }
}
