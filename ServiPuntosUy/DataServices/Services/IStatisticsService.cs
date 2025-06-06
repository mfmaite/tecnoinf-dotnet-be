namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de estadísticas
    /// </summary>
    public interface IStatisticsService
    {
        /// <summary>
        /// Obtiene las estadísticas según el tipo de usuario
        /// </summary>
        /// <returns>Estadísticas adaptadas al tipo de usuario</returns>
        Task<object> GetStatisticsAsync();
    }
}
