using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices.Services.EndUser;

[ApiController]
[Route("api/[controller]")]
public class VEAIController : ControllerBase
{
    private readonly VEAIService _veaiService;

    public VEAIController()
    {
        _veaiService = new VEAIService();
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PersonaRequest request)
    {
        var result = await _veaiService.ObtenerNombrePersona(request.NroDocumento);

        return Ok(result);
    }
}
