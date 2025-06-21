namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de items de transacciones
    /// </summary>
    public class TransactionItemDTO
    {
        /// <summary>
        /// ID del item
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID de la transacción
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// ID del producto
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Cantidad de productos
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del producto
        /// </summary>
        public decimal UnitPrice { get; set; }
        
        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string ProductName { get; set; }
        
        /// <summary>
        /// URL de la imagen del producto
        /// </summary>
        public string ProductImageUrl { get; set; }
        
        /// <summary>
        /// Precio original antes del descuento
        /// </summary>
        public decimal OriginalPrice { get; set; }
        
        /// <summary>
        /// ID de la promoción aplicada (si existe)
        /// </summary>
        public int? PromotionId { get; set; }
        
        /// <summary>
        /// Descripción de la promoción aplicada
        /// </summary>
        public string PromotionDescription { get; set; }
        
        /// <summary>
        /// Indica si el item tiene una promoción aplicada
        /// </summary>
        public bool HasPromotion => PromotionId.HasValue;
        
        /// <summary>
        /// Monto del descuento aplicado
        /// </summary>
        public decimal Discount => HasPromotion ? OriginalPrice - UnitPrice : 0;
        
        /// <summary>
        /// Porcentaje del descuento aplicado
        /// </summary>
        public decimal DiscountPercentage => HasPromotion && OriginalPrice > 0 ? (Discount / OriginalPrice) * 100 : 0;
    }
}
