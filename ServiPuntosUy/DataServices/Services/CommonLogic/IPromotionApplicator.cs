using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para el servicio de aplicación de promociones
    /// </summary>
    public interface IPromotionApplicator
    {
        /// <summary>
        /// Obtiene el precio promocional para un producto en una sucursal específica
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="originalPrice">Precio original del producto</param>
        /// <returns>Precio promocional si hay una promoción aplicable, o el precio original si no hay promoción</returns>
        Task<decimal> GetPromotionalPrice(int productId, int branchId, int tenantId, decimal originalPrice);

        /// <summary>
        /// Obtiene la promoción aplicable para un producto en una sucursal específica
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Promoción aplicable, o null si no hay promoción</returns>
        Task<Promotion> GetApplicablePromotion(int productId, int branchId, int tenantId);
    }
}
