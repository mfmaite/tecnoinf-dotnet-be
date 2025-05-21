using System.Text.Json.Serialization;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DTO
{
    /// <summary>
    /// DTO para transferir información de usuario
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Contraseña del usuario (solo para solicitudes de autenticación)
        /// </summary>
        [JsonIgnore]
        public string? Password { get; set; }
        
        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// ID del tenant al que pertenece el usuario
        /// </summary>
        public string? TenantId { get; set; }
        
        /// <summary>
        /// Tipo de usuario
        /// </summary>
        public UserType UserType { get; set; }
        
        /// <summary>
        /// ID de la estación a la que pertenece el usuario (solo para administradores de estación)
        /// </summary>
        public int? BranchId { get; set; }
    }
}
