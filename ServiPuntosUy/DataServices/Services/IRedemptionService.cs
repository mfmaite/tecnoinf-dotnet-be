using ServiPuntosUy.DTO;
using System.Threading.Tasks;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de canjes de productos
    /// </summary>
    public interface IRedemptionService
    {
        /// <summary>
        /// Genera un token JWT con la información del canje
        /// </summary>
        /// <param name="userId">ID del usuario que realiza el canje</param>
        /// <param name="branchId">ID de la sucursal donde se realiza el canje</param>
        /// <param name="productId">ID del producto a canjear</param>
        /// <returns>Token JWT con la información del canje</returns>
        Task<string> GenerateRedemptionToken(int userId, int branchId, int productId);
        
        /// <summary>
        /// Procesa un canje a partir de un token
        /// </summary>
        /// <param name="token">Token JWT con la información del canje</param>
        /// <returns>Información de la transacción creada</returns>
        Task<TransactionDTO> ProcessRedemption(string token);
    }
}
