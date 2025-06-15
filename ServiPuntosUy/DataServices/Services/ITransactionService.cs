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
        Task<TransactionDTO> CreateTransaction(int userId, int branchId, ProductQuantity[] products);
        Task<TransactionDTO> GetTransactionById(int id);
        Task<TransactionDTO[]> GetTransactionsByUserId(int userId);
        
        /// <summary>
        /// Obtiene los items (productos) de una transacción específica
        /// </summary>
        /// <param name="transactionId">ID de la transacción</param>
        /// <returns>Array de items de la transacción</returns>
        Task<TransactionItemDTO[]> GetTransactionItems(int transactionId);
    }
}
