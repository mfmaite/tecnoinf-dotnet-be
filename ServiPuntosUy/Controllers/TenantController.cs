using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DAO.Models.Central;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Requests;

namespace ServiPuntosUy.Controllers;

/// <summary>
/// Controlador para la gestión de Tenants
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TenantController : BaseController
{
    public TenantController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Crea un nuevo tenant
    /// </summary>
    /// <param name="tenant">Datos del tenant a crear</param>
    /// <returns>El tenant creado</returns>
    /// <response code="200">Retorna el tenant creado</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(TenantDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult CreateTenant([FromBody] Tenant tenant) {
        try {
            var newTenant = TenantService.CreateTenant(
                tenant.Name
            );

            return Ok(new ApiResponse<TenantDTO>{
                Error = false,
                Message = "Tenant creado correctamente",
                Data = newTenant
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

    /// <summary>
    /// Obtiene un tenant por su ID
    /// </summary>
    /// <param name="id">ID del tenant</param>
    /// <returns>El tenant solicitado</returns>
    /// <response code="200">Retorna el tenant solicitado</response>
    /// <response code="404">Si el tenant no existe</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantDTO), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetTenant(int id)
    {
        try
        {
            var tenant = TenantService.GetTenantById(id);
            if (tenant == null)
                return NotFound(new ApiResponse<object>{
                    Error = true,
                    Message = $"No existe un tenant con el ID {id}"
                });

            return Ok(new ApiResponse<TenantDTO>{
                Error = false,
                Message = "Tenant encontrado correctamente",
                Data = tenant
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

    /// <summary>
    /// Obtiene la lista de tenants
    /// </summary>
    /// <returns>La lista de tenants</returns>
    /// <response code="200">Retorna la lista de tenants</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(TenantDTO[]), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetTenantsList()
    {
        try
        {
            var tenants = TenantService.GetTenantsList();

            return Ok(new ApiResponse<TenantDTO[]>{
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

    /// <summary>
    /// Elimina un tenant
    /// </summary>
    /// <param name="id">ID del tenant</param>
    /// <response code="200">Retorna un mensaje de éxito</response>
    /// <response code="404">Si el tenant no existe</response>
    /// <response code="400">Si hay un error en la eliminación</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult DeleteTenant(int id)
    {
        try
        {
            TenantService.DeleteTenant(id);

            return Ok(new ApiResponse<string>{
                Error = false,
                Message = "Tenant eliminado correctamente"
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

    /// <summary>
    /// Modifica un tenant
    /// </summary>
    /// <param name="id">ID del tenant</param>
    /// <response code="200">Retorna el tenant modificado</response>
    /// <response code="404">Si el tenant no existe</response>
    /// <response code="400">Si hay un error en la modificación</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TenantDTO), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult UpdateTenant(int id, [FromBody] UpdateTenantRequest request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new ApiResponse<object>{
                    Error = true,
                    Message = "El nombre del tenant es requerido"
                });
            }

            var updatedTenant = TenantService.UpdateTenant(id, request.Name);

            return Ok(new ApiResponse<TenantDTO>{
                Error = false,
                Message = "Tenant modificado correctamente",
                Data = updatedTenant
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
