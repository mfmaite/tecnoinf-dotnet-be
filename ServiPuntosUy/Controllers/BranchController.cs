using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Requests;

namespace ServiPuntosUy.Controllers;



[Route("api/[controller]")]
[ApiController]
public class BranchController : BaseController
{

    public BranchController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Crear un nuevo branch
    /// </summary>
    /// <param name="branch">Datos del branch a crear</param>
    /// <returns>El branch creado</returns>
    /// <response code="200">Retorna el branch creado</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(BranchDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult CreateBranch([FromBody] CreateBranchRequest request) {
        try {
            var newBranch = BranchService.CreateBranch(
                request.Latitud,
                request.Longitud,
                request.TenantId
            );

            return Ok(newBranch);
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
