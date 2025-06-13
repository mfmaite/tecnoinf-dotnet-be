using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests
{
    /// <summary>
    /// Solicitud para generar un token de canje
    /// </summary>
    public class GenerateRedemptionTokenRequest
    {
        /// <summary>
        /// ID de la sucursal donde se realizará el canje
        /// </summary>
        [Required]
        public int BranchId { get; set; }

        /// <summary>
        /// ID del producto a canjear
        /// </summary>
        [Required]
        public int ProductId { get; set; }
    }

    /// <summary>
    /// Respuesta con el token de canje generado
    /// </summary>
    public class RedemptionTokenResponse
    {
        /// <summary>
        /// Token JWT con la información del canje
        /// </summary>
        public string Token { get; set; }
    }
}
