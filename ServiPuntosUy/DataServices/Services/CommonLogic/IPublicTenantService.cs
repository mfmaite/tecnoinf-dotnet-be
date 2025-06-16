using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para el servicio público de tenants (sin autenticación)
    /// </summary>
    public interface IPublicTenantService
    {
        /// <summary>
        /// Obtiene la lista de tenants
        /// </summary>
        /// <returns>Lista de tenants</returns>
        TenantDTO[] GetTenantsList();
    }
}
