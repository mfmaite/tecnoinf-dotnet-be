using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Enums;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Controllers.Requests;

namespace ServiPuntosUy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GeneralParameterController : BaseController
    {
        public GeneralParameterController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        /// <summary>
        /// Obtiene todos los parámetros generales
        /// </summary>
        /// <returns>Lista de parámetros generales</returns>
        /// <response code="200">Retorna la lista de parámetros generales</response>
        /// <response code="400">Si hay un error en la obtención</response>
        [HttpGet]
        [ProducesResponseType(typeof(GeneralParameterDTO[]), 200)]
        public ActionResult<GeneralParameterDTO[]> GetAllParameters()
        {
            try
            {
                var parameters = GeneralParameterService.GetAllParameters();
                return Ok(new ApiResponse<GeneralParameterDTO[]> 
                { 
                    Error = false, 
                    Data = parameters, 
                    Message = "Parámetros obtenidos con éxito" 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<GeneralParameterDTO[]>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene un parámetro general por su clave
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <returns>Parámetro general</returns>
        /// <response code="200">Retorna el parámetro general</response>
        /// <response code="400">Si hay un error en la obtención</response>
        /// <response code="404">Si no existe un parámetro con la clave especificada</response>
        [HttpGet("{key}")]
        [ProducesResponseType(typeof(GeneralParameterDTO), 200)]
        public ActionResult<GeneralParameterDTO> GetParameter(string key)
        {
            try
            {
                var parameter = GeneralParameterService.GetParameter(key);
                return Ok(new ApiResponse<GeneralParameterDTO> 
                { 
                    Error = false, 
                    Data = parameter, 
                    Message = "Parámetro obtenido con éxito" 
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza un parámetro general existente (solo para Central Admin)
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <param name="value">Nuevo valor</param>
        /// <param name="description">Nueva descripción (opcional)</param>
        /// <returns>Parámetro general actualizado</returns>
        /// <response code="200">Retorna el parámetro general actualizado</response>
        /// <response code="400">Si hay un error en la actualización</response>
        /// <response code="403">Si el usuario no tiene permisos para actualizar parámetros</response>
        /// <response code="404">Si no existe un parámetro con la clave especificada</response>
        [HttpPut("{key}")]
        [ProducesResponseType(typeof(GeneralParameterDTO), 200)]
        public ActionResult<GeneralParameterDTO> UpdateParameter(string key, [FromBody] UpdateParameterRequest request)
        {
            try
            {
                // Verificar que el usuario sea Central Admin
                var userType = ObtainUserTypeFromToken();
                if (userType != UserType.Central)
                {
                    return Forbid();
                }

                var parameter = GeneralParameterService.UpdateParameter(key, request.Value, request.Description);
                return Ok(new ApiResponse<GeneralParameterDTO> 
                { 
                    Error = false, 
                    Data = parameter, 
                    Message = "Parámetro actualizado con éxito" 
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Crea un nuevo parámetro general (solo para Central Admin)
        /// </summary>
        /// <param name="request">Datos del nuevo parámetro</param>
        /// <returns>Parámetro general creado</returns>
        /// <response code="201">Retorna el parámetro general creado</response>
        /// <response code="400">Si hay un error en la creación</response>
        /// <response code="403">Si el usuario no tiene permisos para crear parámetros</response>
        [HttpPost]
        [ProducesResponseType(typeof(GeneralParameterDTO), 201)]
        public ActionResult<GeneralParameterDTO> CreateParameter([FromBody] CreateParameterRequest request)
        {
            try
            {
                // Verificar que el usuario sea Central Admin
                var userType = ObtainUserTypeFromToken();
                if (userType != UserType.Central)
                {
                    return Forbid();
                }

                var parameter = GeneralParameterService.CreateParameter(request.Key, request.Value, request.Description);
                return Created($"/api/generalparameter/{request.Key}", new ApiResponse<GeneralParameterDTO> 
                { 
                    Error = false, 
                    Data = parameter, 
                    Message = "Parámetro creado con éxito" 
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<GeneralParameterDTO>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }
    }
}
