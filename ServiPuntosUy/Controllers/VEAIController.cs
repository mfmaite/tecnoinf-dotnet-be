using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices.Services.EndUser;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DAO.Models.Central;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
namespace ServiPuntosUy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VEAIController : BaseController
{
    public VEAIController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PersonaRequest request)
    {
        try {
            var user = ObtainUserFromToken();
            var result = await VEAIService.VerificarIdentidad(user.Id, request.NroDocumento);


            return Ok(new ApiResponse<UserDTO>{
                Error = false,
                Message = "Verificación de edad exitosa",
                Data = result
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<string>{
                Error = true,
                Message = ex.Message
            });
        }
    }
}
