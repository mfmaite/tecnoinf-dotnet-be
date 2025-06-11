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
    }
}
