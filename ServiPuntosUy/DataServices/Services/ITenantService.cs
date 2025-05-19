using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models;

namespace ServiPuntosUy.DataServices.Services;

public interface ITenantService
{
    TenantDTO GetTenantDTO(Tenant tenant);
    TenantDTO GetTenantById(int id);
    TenantDTO CreateTenant(string name, string tenantId);
}
