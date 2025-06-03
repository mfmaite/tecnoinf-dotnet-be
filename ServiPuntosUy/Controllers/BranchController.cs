using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DataServices.Services.Branch;

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
            // Intentar parsear OpenTime y ClosingTime
            if (!TimeOnly.TryParse(request.OpenTime, out var openTime))
                return BadRequest("Formato de hora inválido para OpenTime. Use HH:mm.");

            if (!TimeOnly.TryParse(request.ClosingTime, out var closingTime))
                return BadRequest("Formato de hora inválido para ClosingTime. Use HH:mm.");

            var loggedUser = ObtainUserFromToken();

            var newBranch = TenantBranchService?.CreateBranch(
                int.Parse(loggedUser.TenantId),
                request.Latitud,
                request.Longitud,
                request.Address,
                request.Phone,
                openTime,
                closingTime
            );

            if (newBranch == null)
            {
                return BadRequest(new ApiResponse<object>{
                    Error = true,
                    Message = "No se pudo crear el branch. El servicio no está disponible."
                });
            }

            return Ok(new ApiResponse<BranchDTO>{
                Error = false,
                Message = "Branch creado correctamente",
                Data = newBranch
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Actualizar un branch
    /// </summary>
    /// <param name="branch">Datos del branch a actualizar</param>
    /// <returns>El branch actualizado</returns>
    /// <response code="200">Retorna el branch creado</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPatch("{id}/Update")]
    [ProducesResponseType(typeof(BranchDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult UpdateBranch(int id, [FromBody] UpdateBranchRequest request) {
        try {
            // Intentar parsear OpenTime y ClosingTime
            TimeOnly? openTime = null;
            if (!string.IsNullOrWhiteSpace(request.OpenTime))
            {
                if (!TimeOnly.TryParse(request.OpenTime, out var parsedOpenTime))
                    return BadRequest("Formato de hora inválido para OpenTime. Use HH:mm.");
                openTime = parsedOpenTime;
            }

            TimeOnly? closingTime = null;
            if (!string.IsNullOrWhiteSpace(request.ClosingTime))
            {
                if (!TimeOnly.TryParse(request.ClosingTime, out var parsedClosingTime))
                    return BadRequest("Formato de hora inválido para ClosingTime. Use HH:mm.");
                closingTime = parsedClosingTime;
            }

            var newBranch = TenantBranchService?.UpdateBranch(
                id,
                request.Latitud ?? null,
                request.Longitud ?? null,
                request.Address ?? null,
                request.Phone ?? null,
                openTime,
                closingTime
            );

            if (newBranch == null)
            {
                return BadRequest(new ApiResponse<object>{
                    Error = true,
                    Message = "No se pudo actualizar el branch. El servicio no está disponible."
                });
            }

            return Ok(new ApiResponse<BranchDTO>{
                Error = false,
                Message = "Branch actualizado correctamente",
                Data = newBranch
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Eliminar un branch
    /// </summary>
    /// <param name="branch">Id del branch a eliminar</param>
    /// <returns>El id del branch eliminado</returns>
    /// <response code="200">OK</response>
    /// <response code="400">Si hay un error en la eliminación</response>
    [HttpDelete("{id}/Delete")]
    [ProducesResponseType(typeof(BranchDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult UpdateBranch(int id) {
        try {
            TenantBranchService?.DeleteBranch(id);
            return Ok(new ApiResponse<object>{
                Error = false,
                Message = "Branch eliminado correctamente"
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }

        /// <summary>
    /// Eliminar un branch
    /// </summary>
    /// <param name="branch">Id del branch a eliminar</param>
    /// <returns>El id del branch eliminado</returns>
    /// <response code="200">OK</response>
    /// <response code="400">Si hay un error en la eliminación</response>
    [HttpPost("hours/{id}")]
    [ProducesResponseType(typeof(BranchDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult setBranchHours(int id, [FromBody] SetBranchHoursRequest request) {
        try {
            // Intentar parsear OpenTime y ClosingTime
            if (!TimeOnly.TryParse(request.OpenTime, out var openTime))
                return BadRequest("Formato de hora inválido para OpenTime. Use HH:mm.");

            if (!TimeOnly.TryParse(request.ClosingTime, out var closingTime))
                return BadRequest("Formato de hora inválido para ClosingTime. Use HH:mm.");
                
            var branch = TenantBranchService?.setBranchHours(id, openTime, closingTime) ;
            return Ok(new ApiResponse<BranchDTO>{
                Error = false,
                Message = "Horario del branch actualizado correctamente",
                Data = branch
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }
    /// <summary>
    /// Obtiene la lista de tenants
    /// </summary>
    /// <returns>La lista de tenants</returns>
    /// <response code="200">Retorna la lista de tenants</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(BranchDTO[]), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetBranchList()
    {
        try
        {
            var tenantId = ObtainUserFromToken().TenantId;
            if (!int.TryParse(tenantId, out int tenantIdInt))
                return BadRequest("No se pudo obtener el TenantId del usuario");
                
            var user = ObtainUserFromToken();
            if (!HasAccessToTenant(user.TenantId))
                return Unauthorized(new ApiResponse<object>{
                    Error = true,
                    Message = "No tiene acceso al tenant especificado"
                });

            var branchList = TenantBranchService.GetBranchList(tenantIdInt);


            return Ok(new ApiResponse <BranchDTO[]>{
                Error = false,
                Message = "Lista de tenants obtenida correctamente",
                Data = branchList
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }
}
