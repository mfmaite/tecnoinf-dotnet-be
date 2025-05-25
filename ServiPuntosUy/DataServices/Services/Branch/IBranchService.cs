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
        BranchDTO GetBranchById(int id);
        BranchDTO CreateBranch(int tenantId, string latitud, string longitud, string address, string phone, TimeOnly openTime, TimeOnly closingTime);
    }

}
