// using Microsoft.AspNetCore.Mvc;
// using ServiPuntosUy.DataServices.Services.Central;
// using ServiPuntosUy.DAO.Models.Central;
// using ServiPuntosUy.DataServices.Services;
// using ServiPuntosUy.DTO;

// namespace ServiPuntosUy.Controllers;

// [Route("api/[controller]")]
// [ApiController]
// public class EstacionController : ControllerBase
// {
//     private readonly IEstacionService _estacionService;

//     public EstacionController(IEstacionService estacionService)
//     {
//         _estacionService = estacionService;
//     }

//     /// <summary>
//     /// Crear una nueva estación
//     /// </summary>
//     /// <param name="estacion">Datos de la estación a crear</param>
//     /// <returns>La estación creada</returns>
//     /// <response code="200">Retorna la estación creada</response>
//     /// <response code="400">Si hay un error en la creación</response>
//     [HttpPost("Create")]
//     [ProducesResponseType(typeof(EstacionDTO), 200)]
//     [ProducesResponseType(400)]
//     public IActionResult CreateEstacion([FromBody] Estacion estacion) {
//         try {
//             var newEstacion = _estacionService.CreateEstacion(
//                 estacion.Latitud,
//                 estacion.Longitud,
//                 estacion.TenantId
//             );

//             return Ok(newEstacion);
//         }
//         catch (Exception ex) {
//             return BadRequest(ex.Message);
//         }
//     }
// }
