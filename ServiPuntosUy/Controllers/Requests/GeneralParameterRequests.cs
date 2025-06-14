namespace ServiPuntosUy.Controllers.Requests
{
    /// <summary>
    /// Solicitud para actualizar un parámetro general
    /// </summary>
    public class UpdateParameterRequest
    {
        /// <summary>
        /// Nuevo valor del parámetro
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Nueva descripción del parámetro (opcional)
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Solicitud para crear un nuevo parámetro general
    /// </summary>
    public class CreateParameterRequest
    {
        /// <summary>
        /// Clave del parámetro
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Valor del parámetro
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Descripción del parámetro
        /// </summary>
        public string Description { get; set; }
    }
}
