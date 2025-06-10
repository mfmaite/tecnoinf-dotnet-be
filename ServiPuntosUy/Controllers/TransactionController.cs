using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Requests;
namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TransactionController : BaseController
{
    public TransactionController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Crear una nueva transacción
    /// </summary>
    /// <param name="request">Datos de la transacción</param>
    /// <returns>La transacción creada con sus items</returns>
    /// <response code="200">Retorna la transacción creada con sus items</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(TransactionResponseDTO), 200)]
    public async Task<ActionResult<TransactionResponseDTO>> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var loggedUser = ObtainUserFromToken();

            var transaction = await TransactionService.CreateTransaction(
                loggedUser.Id,
                request.BranchId,
                int.Parse(loggedUser.TenantId),
                request.ProductIds
            );

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
