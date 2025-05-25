using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.Tenant
{
    public interface ITenantBranchService
    {
        BranchDTO CreateBranch(int tenantId, string latitud, string longitud, string address, string phone, TimeOnly openTime, TimeOnly closingTime);
    }
}
