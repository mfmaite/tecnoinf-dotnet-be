using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUY.Controllers.Response;

namespace ServiPuntosUy.Controllers;

/// <summary>
/// Controlador público para la gestión de Tenants (sin autenticación)
/// </summary>
[Route("api/public/tenant")]
[ApiController]
public class PublicTenantController : ControllerBase
{
    private readonly IPublicTenantService _publicTenantService;

    public PublicTenantController(IServiceFactory serviceFactory)
    {
        _publicTenantService = serviceFactory.GetService<IPublicTenantService>();
    }

    /// <summary>
    /// Obtiene la lista de tenants (endpoint público)
    /// </summary>
    /// <returns>La lista de tenants (solo nombres, sin IDs)</returns>
    /// <response code="200">Retorna la lista de tenants</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(PublicTenantDTO[]), 200)]
    [ProducesResponseType(400)]
    public IActionResult GetTenantsList()
    {
        try
        {
            var tenants = _publicTenantService.GetTenantsList();

            return Ok(new ApiResponse<PublicTenantDTO[]>{
                Error = false,
                Message = "Lista de tenants obtenida correctamente",
                Data = tenants
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }
}
