using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.Branch
{
    /// <summary>
    /// Interfaz para el servicio de branches (exclusivo para el administrador de branch)
    /// </summary>
    public interface IBranchService
    {
        BranchDTO GetBranchDTO(ServiPuntosUy.DAO.Models.Central.Branch branch);
        Task<BranchDTO?> GetBranchById(int id);
        Task<ProductStockDTO?> manageStock(int productId, int branchId, int stock);
        Task<BranchDTO?> setBranchHours(int id,  TimeOnly openTime, TimeOnly closingTime);
    }

}
