

namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de configuración de fidelización
    /// </summary>
    public class LoyaltyConfigDTO
    {
        /// <summary>
        /// ID de la configuración
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID del tenant
        /// </summary>
        public int TenantId { get; set; } = 0;

        /// <summary>
        /// Nombre de los puntos (ej. "Estrellas", "Puntos", etc.)
        /// </summary>
        public string PointsName { get; set; } = string.Empty;

        /// <summary>
        /// Valor de los puntos
        /// </summary>
        public int PointsValue { get; set; } = 1;

        /// <summary>
        /// Regla de acumulación de puntos (cantidad de pesos para acumular un punto)
        /// </summary>
        public decimal AccumulationRule { get; set; } = 1m;

        /// <summary>
        /// Días de expiración de los puntos desde el último login
        /// </summary>
        public int ExpiricyPolicyDays { get; set; } = 180;
    }
}
