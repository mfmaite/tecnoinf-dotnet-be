namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Interfaz para acceder al tenant actual
    /// </summary>
    public interface ITenantAccessor
    {
        /// <summary>
        /// Obtiene el ID del tenant actual
        /// </summary>
        /// <returns>ID del tenant actual</returns>
        string GetCurrentTenantId();
    }
}
