using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;

namespace ServiPuntosUy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoyaltyProgramController : BaseController
    {
        public LoyaltyProgramController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        /// <summary>
        /// Obtiene el programa de fidelidad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del </param>
        /// <returns>El programa de fidelidad del tenant</returns>
        [HttpGet("{tenantId}/program")]
        [ProducesResponseType(typeof(LoyaltyConfigDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public IActionResult GetLoyaltyProgram(int tenantId)
        {
            try {
                var program = LoyaltyService.GetLoyaltyProgram(tenantId);
                return Ok(new ApiResponse<LoyaltyConfigDTO>
                {
                    Error = false,
                    Message = "Programa de fidelidad obtenido correctamente",
                    Data = program
                });
            } catch (Exception ex) {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }
    }
}
