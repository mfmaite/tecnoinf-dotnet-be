using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using ServiPuntosUY.Controllers.Response;

namespace ServiPuntosUy.Controllers
{
    /// <summary>
    /// Controlador para la gestión de UI de tenants
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantUIController : BaseController
    {
        private readonly ITenantUIService _tenantUIService;

        public TenantUIController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
            _tenantUIService = serviceFactory.GetService<ITenantUIService>();
        }

        /// <summary>
        /// Obtiene la UI de un tenant por su ID
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>La UI del tenant solicitado</returns>
        /// <response code="200">Retorna la UI del tenant solicitado</response>
        /// <response code="404">Si la UI del tenant no existe</response>
        /// <response code="400">Si hay un error en la búsqueda</response>
        [HttpGet("{tenantId}")]
        [ProducesResponseType(typeof(TenantUIDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetTenantUI(int tenantId)
        {
            try
            {
                var tenantUI = await _tenantUIService.GetTenantUIAsync(tenantId);

                return Ok(new ApiResponse<TenantUIDTO>
                {
                    Error = false,
                    Message = "UI del tenant obtenida correctamente",
                    Data = tenantUI
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza la UI de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="request">Datos de la UI a actualizar</param>
        /// <returns>La UI del tenant actualizada</returns>
        /// <response code="200">Retorna la UI del tenant actualizada</response>
        /// <response code="403">Si el usuario no tiene permisos para actualizar la UI</response>
        /// <response code="404">Si la UI del tenant no existe</response>
        /// <response code="400">Si hay un error en la actualización</response>
        [HttpPut("{tenantId}")]
        [ProducesResponseType(typeof(TenantUIDTO), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateTenantUI(int tenantId, [FromBody] UpdateTenantUIRequest request)
        {
            try
            {
                var tenantUI = await _tenantUIService.UpdateTenantUIAsync(
                    tenantId,
                    request.LogoUrl,
                    request.PrimaryColor,
                    request.SecondaryColor,
                    HttpContext);

                return Ok(new ApiResponse<TenantUIDTO>
                {
                    Error = false,
                    Message = "UI del tenant actualizada correctamente",
                    Data = tenantUI
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }
    }
}
