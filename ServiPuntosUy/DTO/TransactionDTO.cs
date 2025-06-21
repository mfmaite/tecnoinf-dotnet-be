using ServiPuntosUy.Enums;
using System.Collections.Generic;

namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de transacciones
    /// </summary>
    public class TransactionDTO
    {
        /// <summary>
        /// ID de la transacción
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID de la sucursal
        /// </summary>
        public int BranchId { get; set; }

        /// <summary>
        /// ID del usuario que realizó la transacción
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Fecha de creación de la transacción
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Monto de la transacción
        /// </summary>
        public decimal Amount { get; set; } = 0m;

    /// <summary>
    /// Puntos ganados en la transacción
    /// </summary>
    public int PointsEarned { get; set; } = 0;

    /// <summary>
    /// Tipo de transacción (compra o canje)
    /// </summary>
    public TransactionType Type { get; set; } = TransactionType.Purchase;

    /// <summary>
    /// Puntos gastados en la transacción (para canjes)
    /// </summary>
    public int PointsSpent { get; set; } = 0;
    
    /// <summary>
    /// Lista de items de la transacción
    /// </summary>
    public List<TransactionItemDTO> Items { get; set; } = new List<TransactionItemDTO>();
    
    /// <summary>
    /// Indica si la transacción tiene alguna promoción aplicada
    /// </summary>
    public bool HasPromotions => Items?.Any(i => i.HasPromotion) ?? false;
    
    /// <summary>
    /// Monto total de descuentos aplicados
    /// </summary>
    public decimal TotalDiscount => Items?.Sum(i => i.Discount * i.Quantity) ?? 0;
    
    /// <summary>
    /// Monto original antes de descuentos
    /// </summary>
    public decimal OriginalAmount => Items?.Sum(i => i.OriginalPrice * i.Quantity) ?? Amount;
    }
}
