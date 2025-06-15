using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de parámetros generales
    /// </summary>
    public interface IGeneralParameterService
    {
        /// <summary>
        /// Obtiene un parámetro general por su clave
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <returns>DTO del parámetro general</returns>
        GeneralParameterDTO GetParameter(string key);

        /// <summary>
        /// Obtiene todos los parámetros generales
        /// </summary>
        /// <returns>Array de DTOs de parámetros generales</returns>
        GeneralParameterDTO[] GetAllParameters();

        /// <summary>
        /// Actualiza un parámetro general existente
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <param name="value">Nuevo valor</param>
        /// <param name="description">Nueva descripción (opcional)</param>
        /// <returns>DTO del parámetro actualizado</returns>
        GeneralParameterDTO UpdateParameter(string key, string value, string description = null);

        /// <summary>
        /// Crea un nuevo parámetro general
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <param name="value">Valor del parámetro</param>
        /// <param name="description">Descripción del parámetro</param>
        /// <returns>DTO del parámetro creado</returns>
        GeneralParameterDTO CreateParameter(string key, string value, string description);
    }
}
