using System.Security.Claims;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para la lógica de autenticación común a todos los servicios
    /// </summary>
    public interface IAuthLogic
    {
        /// <summary>
        /// Genera un hash para una contraseña
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="passwordSalt">Salt generado</param>
        /// <returns>Hash de la contraseña</returns>
        string HashPassword(string password, out string passwordSalt);
        
        /// <summary>
        /// Verifica si una contraseña coincide con un hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="passwordHash">Hash de la contraseña</param>
        /// <param name="passwordSalt">Salt utilizado para generar el hash</param>
        /// <returns>true si la contraseña coincide, false en caso contrario</returns>
        bool VerifyPassword(string password, string passwordHash, string passwordSalt);
        
        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>true si el token es válido, false en caso contrario</returns>
        bool ValidateToken(string token);
        
        /// <summary>
        /// Obtiene los claims de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Claims del token</returns>
        IEnumerable<Claim> GetTokenClaims(string token);
        
        /// <summary>
        /// Obtiene un claim específico de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="claimType">Tipo de claim</param>
        /// <returns>Valor del claim, o null si no existe</returns>
        string GetTokenClaim(string token, string claimType);
    }
}
