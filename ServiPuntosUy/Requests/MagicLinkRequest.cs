using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests
{
    /// <summary>
    /// Modelo de request para generar un magic link
    /// </summary>
    public class MagicLinkRequest
    {
        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }
    }

    /// <summary>
    /// Modelo de request para validar un magic link
    /// </summary>
    public class ValidateMagicLinkRequest
    {
        /// <summary>
        /// Token del magic link
        /// </summary>
        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; }
    }
}
