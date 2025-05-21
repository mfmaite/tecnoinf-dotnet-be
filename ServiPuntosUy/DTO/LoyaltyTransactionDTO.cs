
namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de transacciones de fidelización
    /// </summary>
    public class LoyaltyTransactionDTO
    {
        /// <summary>
        /// ID de la transacción
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID del usuario
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// ID del tenant
        /// </summary>
        public string TenantId { get; set; } = string.Empty;
        
        /// <summary>
        /// ID de la estación (opcional)
        /// </summary>
        public int? BranchId { get; set; }
        
        /// <summary>
        /// Tipo de transacción (Acumulación, Canje, Expiración, Ajuste)
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;
        
        /// <summary>
        /// Cantidad de puntos (positivo para acumulación, negativo para canje o expiración)
        /// </summary>
        public decimal Points { get; set; }
        
        /// <summary>
        /// Monto de la transacción (para acumulación)
        /// </summary>
        public decimal? Amount { get; set; }
        
        /// <summary>
        /// Descripción de la transacción
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// ID del producto (para canje)
        /// </summary>
        public int? ProductId { get; set; }
        
        /// <summary>
        /// Nombre del producto (para canje)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;
        
        /// <summary>
        /// Código QR para canje
        /// </summary>
        public string QrCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha de expiración del código QR
        /// </summary>
        public DateTime? QrExpirationDate { get; set; }
        
        /// <summary>
        /// Estado de la transacción (Pendiente, Completada, Cancelada, Expirada)
        /// </summary>
        public string Status { get; set; } = "Pendiente";
        
        /// <summary>
        /// Fecha de la transacción
        /// </summary>
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Fecha de expiración de los puntos
        /// </summary>
        public DateTime? ExpirationDate { get; set; }
        
        /// <summary>
        /// Referencia externa (ej. ID de transacción en sistema POS)
        /// </summary>
        public string ExternalReference { get; set; } = string.Empty;
        
        /// <summary>
        /// Datos adicionales de la transacción
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}
