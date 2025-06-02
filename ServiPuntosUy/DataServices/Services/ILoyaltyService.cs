
namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de fidelización
    /// </summary>
    public interface ILoyaltyService
    {
        /// <summary>
        /// Verifica si los puntos del usuario han expirado según la política de expiración
        /// y actualiza la fecha del último login
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si los puntos expiraron, False en caso contrario</returns>
        Task<bool> CheckPointsExpirationAsync(int userId);
        
        // Otros métodos para gestionar el sistema de puntos
    }
}
