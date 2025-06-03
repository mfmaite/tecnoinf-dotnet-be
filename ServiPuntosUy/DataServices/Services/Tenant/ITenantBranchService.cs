using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.Tenant
{
    public interface ITenantBranchService
    {
        BranchDTO CreateBranch(int tenantId, string latitud, string longitud, string address, string phone, TimeOnly openTime, TimeOnly closingTime);
        BranchDTO UpdateBranch(int branchId, string? latitud, string? longitud, string? address, string? phone, TimeOnly? openTime, TimeOnly? closingTime);
        void DeleteBranch(int branchId);
        BranchDTO setBranchHours(int id, string openTime, string closingTime);
        BranchDTO[] GetBranchList(int tenantId);
    }
}
