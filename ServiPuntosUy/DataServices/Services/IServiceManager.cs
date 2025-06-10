using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de servicios
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Crea un nuevo servicio con su disponibilidad
        /// </summary>
        /// <param name="branchId">ID de la branch</param>
        /// <param name="name">Nombre del servicio</param>
        /// <param name="description">Descripción del servicio</param>
        /// <param name="price">Precio del servicio</param>
        /// <param name="ageRestricted">Indica si el servicio tiene restricción de edad</param>
        /// <param name="startTime">Hora de inicio del servicio</param>
        /// <param name="endTime">Hora de fin del servicio</param>
        /// <returns>DTO del servicio creado con su disponibilidad</returns>
        Task<ServiceDTO> CreateServiceAsync(int branchId, string name, string description, decimal price, bool ageRestricted, TimeOnly startTime, TimeOnly endTime);

        /// <summary>
        /// Actualiza un servicio existente y su disponibilidad
        /// </summary>
        /// <param name="serviceId">ID del servicio</param>
        /// <param name="name">Nombre del servicio</param>
        /// <param name="description">Descripción del servicio</param>
        /// <param name="price">Precio del servicio</param>
        /// <param name="ageRestricted">Indica si el servicio tiene restricción de edad</param>
        /// <param name="startTime">Hora de inicio del servicio (opcional)</param>
        /// <param name="endTime">Hora de fin del servicio (opcional)</param>
        /// <returns>DTO del servicio actualizado con su disponibilidad</returns>
        Task<ServiceDTO> UpdateServiceAsync(int serviceId, string name, string description, decimal price, bool ageRestricted, TimeOnly? startTime = null, TimeOnly? endTime = null);

        /// <summary>
        /// Elimina un servicio
        /// </summary>
        /// <param name="serviceId">ID del servicio</param>
        /// <returns>True si se eliminó correctamente, False en caso contrario</returns>
        Task<bool> DeleteServiceAsync(int serviceId);


        /// <summary>
        /// Obtiene los servicios disponibles en una branch, incluyendo su información de disponibilidad
        /// </summary>
        /// <param name="branchId">ID de la branch</param>
        /// <returns>Array de DTOs de servicios con sus disponibilidades</returns>
        Task<ServiceDTO[]> GetBranchServicesAsync(int branchId);

        /// <summary>
        /// Obtiene un servicio específico por su ID, incluyendo su información de disponibilidad
        /// </summary>
        /// <param name="serviceId">ID del servicio</param>
        /// <returns>DTO del servicio con sus disponibilidades</returns>
        Task<ServiceDTO> GetServiceByIdAsync(int serviceId);
    }
}
