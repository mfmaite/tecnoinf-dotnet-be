using ServiPuntosUy.DTO;

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

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        LoyaltyConfigDTO GetLoyaltyProgram(int tenantId);

        /// <summary>
        /// Crea un programa de fidelidad para un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="pointsName">Nombre de los puntos</param>
        /// <param name="pointsValue">Valor de los puntos</param>
        /// <param name="accumulationRule">Regla de acumulación de puntos</param>
        /// <param name="expiricyPolicyDays">Días de expiración de los puntos</param>
        /// <returns>Configuración de lealtad</returns>
        LoyaltyConfigDTO CreateLoyaltyProgram(int tenantId, string pointsName, int pointsValue, decimal accumulationRule, int expiricyPolicyDays);
    }
}
