using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DataServices
{
    /// <summary>
    /// Fábrica de servicios que proporciona acceso a los servicios según el tenant y tipo de usuario
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Obtiene un servicio del tipo especificado
        /// </summary>
        /// <typeparam name="T">Tipo de servicio a obtener</typeparam>
        /// <returns>Instancia del servicio</returns>
        T GetService<T>() where T : class;
        
        /// <summary>
        /// Configura los servicios según el tenant y tipo de usuario
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="userType">Tipo de usuario</param>
        /// <param name="httpContext">Contexto HTTP (opcional)</param>
        void ConfigureServices(string tenantId, UserType userType, HttpContext httpContext = null);
    }
}
