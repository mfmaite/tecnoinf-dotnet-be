using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services;

namespace ServiPuntosUy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController(IServiceFactory serviceFactory) : BaseController(serviceFactory)
    {
        private readonly IStatisticsService _statisticsService = serviceFactory.GetService<IStatisticsService>();

        /// <summary>
        /// Obtiene las estadísticas según el tipo de usuario autenticado
        /// </summary>
        /// <returns>Estadísticas adaptadas al tipo de usuario</returns>
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _statisticsService.GetStatisticsAsync();
                return Ok(new ApiResponse<object>
                {
                    Error = false,
                    Data = statistics,
                    Message = "Estadísticas obtenidas correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }
    }
}
