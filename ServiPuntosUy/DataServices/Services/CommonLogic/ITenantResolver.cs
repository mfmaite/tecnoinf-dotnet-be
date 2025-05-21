using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para resolver el tenant y tipo de usuario a partir del contexto HTTP
    /// </summary>
    public interface ITenantResolver
    {
        /// <summary>
        /// Resuelve el ID del tenant a partir del contexto HTTP
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        /// <returns>ID del tenant</returns>
        string ResolveTenantId(HttpContext context);
        
        /// <summary>
        /// Resuelve el tipo de usuario a partir del contexto HTTP
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        /// <returns>Tipo de usuario</returns>
        UserType ResolveUserType(HttpContext context);
        
        /// <summary>
        /// Resuelve el ID de la estación a partir del contexto HTTP (solo para administradores de estación)
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        /// <returns>ID de la estación, o null si no aplica</returns>
        int? ResolveBranchId(HttpContext context);
    }
}
