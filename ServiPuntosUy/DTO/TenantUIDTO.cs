namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de UI de tenant
    /// </summary>
    public class TenantUIDTO
    {
        /// <summary>
        /// ID de la configuración UI
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// ID del tenant al que pertenece la configuración
        /// </summary>
        public int TenantId { get; set; }
        
        /// <summary>
        /// URL del logo del tenant
        /// </summary>
        public string LogoUrl { get; set; }
        
        /// <summary>
        /// Color primario del tenant
        /// </summary>
        public string PrimaryColor { get; set; }
        
        /// <summary>
        /// Color secundario del tenant
        /// </summary>
        public string SecondaryColor { get; set; }
    }
}
