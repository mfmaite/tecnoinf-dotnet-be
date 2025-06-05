using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests
{
    /// <summary>
    /// Solicitud para actualizar la UI de un tenant
    /// </summary>
    public class UpdateTenantUIRequest
    {
        /// <summary>
        /// URL del logo del tenant
        /// </summary>
        [Required(ErrorMessage = "La URL del logo es requerida")]
        public string LogoUrl { get; set; }
        
        /// <summary>
        /// Color primario del tenant
        /// </summary>
        [Required(ErrorMessage = "El color primario es requerido")]
        public string PrimaryColor { get; set; }
        
        /// <summary>
        /// Color secundario del tenant
        /// </summary>
        [Required(ErrorMessage = "El color secundario es requerido")]
        public string SecondaryColor { get; set; }
    }
}
