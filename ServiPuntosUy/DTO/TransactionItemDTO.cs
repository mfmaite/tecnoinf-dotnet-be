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
    }
}
