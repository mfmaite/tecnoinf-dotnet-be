using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Requests;

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
        /// Crea el programa de fidelidad para un tenant
        /// </summary>
        /// <param name="request">Request con los datos del programa de fidelidad</param>
        /// <returns>El programa de fidelidad del tenant</returns>
        [HttpPost("")]
        [ProducesResponseType(typeof(LoyaltyConfigDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public IActionResult CreateLoyaltyProgram([FromBody] CreateLoyaltyProgramRequest request)
        {
            try {
                var loggedUser = ObtainUserFromToken();

                var program = LoyaltyService.CreateLoyaltyProgram(
                    int.Parse(loggedUser.TenantId),
                    request.PointsName,
                    request.PointsValue,
                    request.AccumulationRule,
                    request.ExpiricyPolicyDays
                );

                return Ok(new ApiResponse<LoyaltyConfigDTO>
                {
                    Error = false,
                    Message = "Programa de fidelidad creado correctamente",
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

        /// <summary>
        /// Obtiene el programa de fidelidad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>El programa de fidelidad del tenant</returns>
        [HttpGet("{tenantId}/program")]
        [ProducesResponseType(typeof(LoyaltyConfigDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public IActionResult GetLoyaltyProgram(int tenantId)
        {
            try {
                var loggedUser = ObtainUserFromToken();

                if (loggedUser.UserType != ServiPuntosUy.Enums.UserType.Central && tenantId != int.Parse(ObtainUserFromToken().TenantId)) {
                    throw new Exception("No tienes permisos para obtener el programa de fidelidad de este tenant");
                }

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

        /// <summary>
        /// Obtiene el programa de fidelidad de el tenant correspondiente al usuario loggeado
        /// </summary>
        /// <returns>El programa de fidelidad del tenant</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(LoyaltyConfigDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public IActionResult GetLoyaltyProgram()
        {
            try {
                var loggedUser = ObtainUserFromToken();
                var program = LoyaltyService.GetLoyaltyProgram(int.Parse(loggedUser.TenantId));
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

        /// <summary>
        /// Actualiza el programa de fidelidad de un tenant
        /// </summary>
        /// <param name="request">Request con los datos del programa de fidelidad</param>
        /// <returns>El programa de fidelidad del tenant</returns>
        [HttpPut("")]
        [ProducesResponseType(typeof(LoyaltyConfigDTO), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public IActionResult UpdateLoyaltyProgram([FromBody] UpdateLoyaltyProgramRequest request)
        {
            try {
                var loggedUser = ObtainUserFromToken();

                var program = LoyaltyService.UpdateLoyaltyProgram(
                    int.Parse(loggedUser.TenantId),
                    request.PointsName,
                    request.PointsValue,
                    request.AccumulationRule,
                    request.ExpiricyPolicyDays
                );

                return Ok(new ApiResponse<LoyaltyConfigDTO>
                {
                    Error = false,
                    Message = "Programa de fidelidad actualizado correctamente",
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
