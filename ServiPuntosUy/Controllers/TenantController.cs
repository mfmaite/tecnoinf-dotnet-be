using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.Controllers;

/// <summary>
/// Controlador para la gestión de Tenants
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
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
            var newTenant = _tenantService.CreateTenant(
                tenant.Name,
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
    /// <param name="tenantId">ID del tenant</param>
    /// <returns>El tenant solicitado</returns>
    /// <response code="200">Retorna el tenant solicitado</response>
    /// <response code="404">Si el tenant no existe</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("{tenantId}")]
    [ProducesResponseType(typeof(TenantDTO), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetTenant(string tenantId)
    {
        try
        {
            var tenant = _tenantService.GetTenantDTOByTenantId(tenantId);
            if (tenant == null)
                return NotFound($"Tenant with ID {tenantId} not found");

            return Ok(tenant);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
