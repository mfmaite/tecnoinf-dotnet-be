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
                request.Products
            );

            return Ok(new ApiResponse<TransactionDTO> { Error = false, Data = transaction, Message = "Transacción creada con exito" });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<TransactionResponseDTO>
            {
                Error = true,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtiene el historial de transacciones del usuario actual
    /// </summary>
    /// <returns>Lista de transacciones del usuario</returns>
    /// <response code="200">Retorna la lista de transacciones</response>
    /// <response code="400">Si hay un error en la obtención</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(TransactionDTO[]), 200)]
    public async Task<ActionResult<TransactionDTO[]>> GetTransactionHistory()
    {
        try
        {
            var loggedUser = ObtainUserFromToken();
            var transactions = await TransactionService.GetTransactionsByUserId(loggedUser.Id);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<TransactionDTO[]>
            {
                Error = true,
                Message = ex.Message
            });
        }
    }
}
