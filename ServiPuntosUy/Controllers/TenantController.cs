using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

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
                tenant.TenantId
            );

            return Ok(newTenant);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
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
                return NotFound($"Tenant with ID {id} not found");

            return Ok(tenant);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
