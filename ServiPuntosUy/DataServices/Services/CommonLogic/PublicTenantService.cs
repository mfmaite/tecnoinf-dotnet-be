using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;
using System.Linq;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación del servicio público de tenants (sin autenticación)
    /// </summary>
    public class PublicTenantService : IPublicTenantService
    {
        private readonly IGenericRepository<DAO.Models.Central.Tenant> _tenantRepository;

        public PublicTenantService(IGenericRepository<DAO.Models.Central.Tenant> tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Convierte un modelo de tenant a DTO público (solo nombre, sin ID)
        /// </summary>
        /// <param name="tenant">Modelo de tenant</param>
        /// <returns>DTO público de tenant</returns>
        private PublicTenantDTO MapToPublicTenantDTO(DAO.Models.Central.Tenant tenant)
        {
            return new PublicTenantDTO
            {
                Name = tenant.Name
            };
        }

        /// <summary>
        /// Obtiene la lista de tenants (solo nombres, sin IDs)
        /// </summary>
        /// <returns>Lista de tenants</returns>
        public PublicTenantDTO[] GetTenantsList()
        {
            var tenants = _tenantRepository.GetQueryable().ToList();
            return tenants.Select(t => MapToPublicTenantDTO(t)).ToArray();
        }
    }
}
