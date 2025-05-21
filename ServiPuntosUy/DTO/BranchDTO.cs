using System.Text.Json.Serialization;

namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de estaciones
    /// </summary>
    public class BranchDTO
    {
        /// <summary>
        /// ID de la estación
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Nombre de la estación
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Dirección de la estación
        /// </summary>
        public string Address { get; set; } = string.Empty;
        
        /// <summary>
        /// Ciudad de la estación
        /// </summary>
        public string City { get; set; } = string.Empty;
        
        /// <summary>
        /// Departamento de la estación
        /// </summary>
        public string State { get; set; } = string.Empty;
        
        /// <summary>
        /// Código postal de la estación
        /// </summary>
        public string ZipCode { get; set; } = string.Empty;
        
        /// <summary>
        /// Latitud de la estación
        /// </summary>
        public double Latitude { get; set; }
        
        /// <summary>
        /// Longitud de la estación
        /// </summary>
        public double Longitude { get; set; }
        
        /// <summary>
        /// Teléfono de la estación
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        
        /// <summary>
        /// Email de la estación
        /// </summary>
        public string Email { get; set; } = string.Empty;
        
        /// <summary>
        /// ID del tenant al que pertenece la estación
        /// </summary>
        public string TenantId { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica si la estación está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Horarios de apertura y cierre por día de la semana
        /// </summary>
        public Dictionary<string, Tuple<TimeSpan, TimeSpan>> OpeningHours { get; set; } = new Dictionary<string, Tuple<TimeSpan, TimeSpan>>();
        
        /// <summary>
        /// Precios de combustibles
        /// </summary>
        public Dictionary<string, decimal> FuelPrices { get; set; } = new Dictionary<string, decimal>();
        
        /// <summary>
        /// Servicios disponibles
        /// </summary>
        public Dictionary<string, bool> Services { get; set; } = new Dictionary<string, bool>();
        
        /// <summary>
        /// URL de la imagen de la estación
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;
        
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
