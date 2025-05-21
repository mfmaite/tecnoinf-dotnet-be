using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DataServices.Services;

public interface ITenantService
{
    TenantDTO GetTenantDTO(Tenant tenant);
    Tenant GetTenantObjectByTenantId(string tenantId);
    TenantDTO GetTenantDTOByTenantId(string tenantId);
    TenantDTO CreateTenant(string name, string tenantId);
    void AddEstacionToTenant(string tenantId, Estacion estacion);
}
