namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación para acceder al tenant actual
    /// </summary>
    public class TenantAccessor : ITenantAccessor
    {
        private readonly string _currentTenantId;
        
        public TenantAccessor(string currentTenantId)
        {
            _currentTenantId = currentTenantId;
        }
        
        /// <summary>
        /// Obtiene el ID del tenant actual
        /// </summary>
        /// <returns>ID del tenant actual</returns>
        public string GetCurrentTenantId()
        {
            return _currentTenantId;
        }
    }
}
