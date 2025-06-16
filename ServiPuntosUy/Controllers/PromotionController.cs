using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using Microsoft.AspNetCore.Authorization;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PromotionController : BaseController
{
    public PromotionController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Crear una nueva promocion
    /// </summary>
    /// <param name="promotion"></param>
    /// <returns>Promocion creada</returns>
    /// <response code="200">Retorna la promocion creada</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(PromotionDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddNewPromotion([FromBody] AddNewPromotionRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = "La solicitud no puede estar vacía."
                });
            
            // // verificamos que la fecha de la promocion sea valida
            if (request.StartDate >= request.EndDate)
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = "La fecha de inicio debe ser anterior a la fecha de fin."
                });
            
            var promocion = await PromotionService.AddPromotion(
                request.tenantId,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.Branch,
                request.Product,
                request.Price
            );


            return Ok(new ApiResponse<PromotionDTO>
            {
                Error = false,
                Message = "Promoción creada correctamente",
                Data = promocion
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

    [HttpPost("Update")]
    [ProducesResponseType(typeof(PromotionDTO), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdatePromotion([FromBody] UpdatePromotionRequest request) {
        try
        {
            
            // verificamos que la fecha de la promocion sea valida
            if (request.StartDate >= request.EndDate)
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");
            
            var promocion = await PromotionService.UpdatePromotion(
                request.PromotionId,
                request.tenantId,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.Branch,
                request.Product,
                request.Price
            );


            return Ok(new ApiResponse<PromotionDTO>
            {
                Error = false,
                Message = "Promoción actualizada correctamente",
                Data = promocion
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
        /// <summary>
    /// Obtiene la lista de promociones
    /// </summary>
    /// <returns>La lista de promociones</returns>
    /// <response code="200">Retorna la lista de promociones</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("tenant")]
    [ProducesResponseType(typeof(PromotionExtendedDTO[]), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetPromotionList()
    {
        try
        {
            // Obtenemos tenant id del usuario loguead
            var tenantId = int.Parse(ObtainUserFromToken().TenantId ?? "0");
            if (tenantId == 0)
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = "No se pudo obtener el tenant ID del token."
                });
            
            var promotionList = PromotionService.GetPromotionList(tenantId);

            return Ok(new ApiResponse<PromotionExtendedDTO[]>
            {
                Error = false,
                Message = "Lista de promociones obtenida correctamente",
                Data = promotionList.ToArray()
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

        /// <summary>
    /// Obtiene una promocion por su ID
    /// </summary>
    /// <returns>La promocion solicitada</returns>
    /// <response code="200">Retorna la promocion solicitada</response>
    /// <response code="404">Si la promocion no existe</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("{promotionId}/{branchId}")]
    [ProducesResponseType(typeof(PromotionExtendedDTO), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetPromotion(int promotionId, int branchId)
    
    {
        try
        {
            var promotion = PromotionService.GetPromotion(promotionId, branchId);
            if (promotion == null)
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = $"No existe un producto con el ID {promotionId}"
                });

            return Ok(new ApiResponse<PromotionExtendedDTO>
            {
                Error = false,
                Message = "Promocion encontrada correctamente",
                Data = promotion
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

    /// <summary>
    /// Crear una nueva promocion para un branch
    /// </summary>
    /// <param name="promotion"></param>
    /// <returns>Promocion creada</returns>
    /// <response code="200">Retorna la promocion creada</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Branch/Create")]
    [ProducesResponseType(typeof(PromotionDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddNewPromotionForBranch([FromBody] AddNewPromotionRequest request)
    {
        try
        {
            // // verificamos que la fecha de la promocion sea valida
            if (request.StartDate >= request.EndDate)
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin.");

            // Obtenemos branch id del usuario loguead
            var branchId = ObtainBranchIdFromToken();
            var tenantId = ObtainTenantFromToken();
            
            var promocion = await PromotionService.AddPromotionForBranch(
                int.Parse(tenantId),
                branchId ?? 0,
                request.Description,
                request.StartDate,
                request.EndDate,
                request.Product,
                request.Price
            );


            return Ok(new ApiResponse<PromotionDTO>
            {
                Error = false,
                Message = "Promoción creada correctamente",
                Data = promocion
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
