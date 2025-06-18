using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using Microsoft.AspNetCore.Http;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de autenticación
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica a un usuario con Google
        /// </summary>
        /// <param name="idToken">Token de ID de Google</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="googleId">ID de Google del usuario</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        Task<UserSessionDTO?> AuthenticateWithGoogleAsync(
            string idToken, 
            string email, 
            string name, 
            string googleId, 
            HttpContext httpContext);
            
        /// <summary>
        /// Autentica a un usuario con sus credenciales
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token JWT si la autenticación es exitosa, null en caso contrario</returns>
        Task<UserSessionDTO?> AuthenticateAsync(string email, string password, HttpContext httpContext);

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Información del usuario</returns>
        Task<UserDTO> GetUserInfoAsync(int userId);

        /// <summary>
        /// Verifica si un token JWT es válido
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>true si el token es válido, false en caso contrario</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Registra a un usuario
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Usuario registrado</returns>
        Task<UserSessionDTO> Signup(string email, string password, string name, int tenantId, HttpContext httpContext);

        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="name">Nombre del usuario</param>
        /// <param name="role">Tipo de usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="googleId">ID de Google del usuario (opcional)</param>
        /// <returns>Token JWT</returns>
        Task<UserSessionDTO> GenerateJwtToken(
            int userId,
            string email,
            string name,
            UserType role,
            int? tenantId,
            int? branchId,
            string googleId = null
        );

        /// <summary>
        /// Genera un magic link para el login
        /// </summary>
        /// <param name="email">Email del usuario</param>
        /// <param name="httpContext">Contexto HTTP para obtener información adicional</param>
        /// <returns>Token del magic link</returns>
        Task<string> GenerateMagicLinkAsync(string email, HttpContext httpContext);

        /// <summary>
        /// Valida un magic link y genera una sesión
        /// </summary>
        /// <param name="magicLinkToken">Token del magic link</param>
        /// <returns>Token de sesión si el magic link es válido</returns>
        Task<UserSessionDTO> ValidateMagicLinkAsync(string magicLinkToken);
    }
}
