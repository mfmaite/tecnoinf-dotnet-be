namespace ServiPuntosUY.Controllers.Response
{
    /// <summary>
    /// Respuesta estándar para las APIs con información de error, datos y mensaje.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica si ocurrió un error.
        /// </summary>
        public bool Error { get; set; } = false;

        /// <summary>
        /// Datos de la respuesta.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Mensaje adicional o de error.
        /// </summary>
        public string? Message { get; set; }
    }
}
