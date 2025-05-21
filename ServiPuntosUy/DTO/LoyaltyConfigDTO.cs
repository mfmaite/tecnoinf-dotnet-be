

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
        public string TenantId { get; set; } = string.Empty;
        
        /// <summary>
        /// Nombre del programa de fidelización
        /// </summary>
        public string ProgramName { get; set; } = string.Empty;
        
        /// <summary>
        /// Nombre de los puntos (ej. "Estrellas", "Puntos", etc.)
        /// </summary>
        public string PointsName { get; set; } = string.Empty;
        
        /// <summary>
        /// Valor de cada punto en moneda local
        /// </summary>
        public decimal PointValue { get; set; } = 1;
        
        /// <summary>
        /// Monto en moneda local necesario para obtener un punto
        /// </summary>
        public decimal AmountPerPoint { get; set; } = 100;
        
        /// <summary>
        /// Reglas de acumulación por tipo de producto
        /// </summary>
        public Dictionary<string, decimal> AccumulationRules { get; set; } = new Dictionary<string, decimal>();
        
        /// <summary>
        /// Días de caducidad de los puntos (0 = no caducan)
        /// </summary>
        public int ExpirationDays { get; set; } = 365;
        
        /// <summary>
        /// Colores del programa de fidelización
        /// </summary>
        public Dictionary<string, string> Colors { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// URL del logo del programa
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Términos y condiciones del programa
        /// </summary>
        public string TermsAndConditions { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica si el programa está activo
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
