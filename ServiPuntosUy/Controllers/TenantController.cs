using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpPost("Create")]
    public IActionResult CreateTenant([FromBody] Tenant tenant) {
      try {
        var newTenant = _tenantService.CreateTenant(
          tenant.Name,
          tenant.DatabaseName,
          tenant.ConnectionString,
          tenant.User,
          tenant.Password
        );

            return Ok(newTenant);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetTenant(int id)
    {
        try
        {
            var tenant = _tenantService.GetTenantById(id);
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
