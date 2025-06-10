using ServiPuntosUy.DTO;
using System.Threading.Tasks;
using ServiPuntosUy.Requests;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de transacciones
    /// </summary>
    public interface ITransactionService
    {
        // Métodos para gestionar transacciones
        Task<TransactionDTO> CreateTransaction(int userId, int branchId, int tenantId, ProductQuantity[] products);
        Task<TransactionDTO> GetTransactionById(int id);
        Task<TransactionDTO[]> GetTransactionsByUserId(int userId);
    }
}
