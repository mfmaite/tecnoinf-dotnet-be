namespace ServiPuntosUY.Controllers.Response
{
    /// <summary>
    /// Response estándar para las APIs con información de error, datos y mensaje.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indica si ocurrió un error.
        /// </summary>
        public bool Error { get; set; } = false;

        /// <summary>
        /// Datos de la respuesta.
        /// </summary>
        public dynamic? Data { get; set; }

        /// <summary>
        /// Mensaje adicional o de error.
        /// </summary>
        public string? Message { get; set; }
    }
}
