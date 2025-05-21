using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services.Branch
{
    /// <summary>
    /// Interfaz para el servicio de estaciones (exclusivo para el administrador de estación)
    /// </summary>
    public interface IBranchService
    {
        /// <summary>
        /// Obtiene todas las estaciones de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Lista de estaciones</returns>
        Task<IEnumerable<BranchDTO>> GetBranchesByTenantAsync(string tenantId);
        
        /// <summary>
        /// Obtiene una estación por su ID
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>Estación</returns>
        Task<BranchDTO> GetBranchByIdAsync(int id);
        
        /// <summary>
        /// Crea una nueva estación
        /// </summary>
        /// <param name="branchDTO">Datos de la estación</param>
        /// <returns>Estación creada</returns>
        Task<BranchDTO> CreateBranchAsync(BranchDTO branchDTO);
        
        /// <summary>
        /// Actualiza una estación
        /// </summary>
        /// <param name="branchDTO">Datos de la estación</param>
        /// <returns>Estación actualizada</returns>
        Task<BranchDTO> UpdateBranchAsync(BranchDTO branchDTO);
        
        /// <summary>
        /// Elimina una estación
        /// </summary>
        /// <param name="id">ID de la estación</param>
        /// <returns>True si la eliminación es exitosa, false en caso contrario</returns>
        Task<bool> DeleteBranchAsync(int id);
        
        /// <summary>
        /// Obtiene las estaciones cercanas a una ubicación
        /// </summary>
        /// <param name="latitude">Latitud</param>
        /// <param name="longitude">Longitud</param>
        /// <param name="radius">Radio en kilómetros</param>
        /// <param name="tenantId">ID del tenant (opcional)</param>
        /// <returns>Lista de estaciones cercanas</returns>
        Task<IEnumerable<BranchDTO>> GetNearbyBranchesAsync(double latitude, double longitude, double radius, string tenantId = null);
        
        /// <summary>
        /// Actualiza los precios de combustibles de una estación
        /// </summary>
        /// <param name="branchId">ID de la estación</param>
        /// <param name="fuelPrices">Precios de combustibles</param>
        /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
        Task<bool> UpdateFuelPricesAsync(int branchId, Dictionary<string, decimal> fuelPrices);
        
        /// <summary>
        /// Actualiza los horarios de una estación
        /// </summary>
        /// <param name="branchId">ID de la estación</param>
        /// <param name="openingHours">Horarios de apertura y cierre</param>
        /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
        Task<bool> UpdateOpeningHoursAsync(int branchId, Dictionary<string, Tuple<TimeSpan, TimeSpan>> openingHours);
        
        /// <summary>
        /// Actualiza la disponibilidad de servicios de una estación
        /// </summary>
        /// <param name="branchId">ID de la estación</param>
        /// <param name="services">Servicios disponibles</param>
        /// <returns>True si la actualización es exitosa, false en caso contrario</returns>
        Task<bool> UpdateServicesAsync(int branchId, Dictionary<string, bool> services);
    }
}
