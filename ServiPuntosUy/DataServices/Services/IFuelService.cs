using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de precios de combustible
    /// </summary>
    public interface IFuelService
    {
        /// <summary>
        /// Actualiza el precio de un combustible para una estación
        /// </summary>
        /// <param name="branchId">ID de la estación</param>
        /// <param name="fuelType">Tipo de combustible</param>
        /// <param name="price">Nuevo precio</param>
        /// <returns>El precio actualizado</returns>
        FuelPrices UpdateFuelPrice(int branchId, FuelType fuelType, decimal price);

        /// <summary>
        /// Obtiene el precio de un combustible para una estación
        /// </summary>
        /// <param name="branchId">ID de la estación</param>
        /// <param name="fuelType">Tipo de combustible</param>
        /// <returns>El precio del combustible</returns>
        FuelPrices GetFuelPrice(int branchId, FuelType fuelType);
    }
}
