using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUy.DataServices.Services.Tenant;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
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
            Console.WriteLine($"TenantId: {request?.TenantId}");
            Console.WriteLine($"Latitud: {request?.Latitud}");
            Console.WriteLine($"Longitud: {request?.Longitud}");
            Console.WriteLine($"Address: {request?.Address}");
            Console.WriteLine($"Phone: {request?.Phone}");
            Console.WriteLine($"OpenTime: {request?.OpenTime}");
            Console.WriteLine($"ClosingTime: {request?.ClosingTime}");

            // Intentar parsear OpenTime y ClosingTime
            if (!TimeOnly.TryParse(request.OpenTime, out var openTime))
                return BadRequest("Formato de hora inválido para OpenTime. Use HH:mm.");

            if (!TimeOnly.TryParse(request.ClosingTime, out var closingTime))
                return BadRequest("Formato de hora inválido para ClosingTime. Use HH:mm.");

            var newBranch = TenantBranchService?.CreateBranch(
                request.TenantId,
                request.Latitud,
                request.Longitud,
                request.Address,
                request.Phone,
                openTime,
                closingTime
            );

            if (newBranch == null)
            {
                return BadRequest("No se pudo crear el branch. El servicio no está disponible.");
            }

            return Ok(newBranch);
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
