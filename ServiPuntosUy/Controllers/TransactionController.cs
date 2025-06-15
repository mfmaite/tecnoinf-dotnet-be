using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Requests;
using System.Threading.Tasks;
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
    [ProducesResponseType(typeof(ApiResponse<TransactionDTO>), 200)]
    public async Task<ActionResult<ApiResponse<TransactionDTO>>> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var loggedUser = ObtainUserFromToken();

            var transaction = await TransactionService.CreateTransaction(
                loggedUser.Id,
                request.BranchId,
                request.Products
            );

            return Ok(new ApiResponse<TransactionDTO>
            {
                Error = false,
                Data = transaction,
                Message = "Transacción creada exitosamente"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<TransactionDTO>
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
    [ProducesResponseType(typeof(ApiResponse<TransactionDTO[]>), 200)]
    public async Task<ActionResult<ApiResponse<TransactionDTO[]>>> GetTransactionHistory()
    {
        try
        {
            var loggedUser = ObtainUserFromToken();
            var transactions = await TransactionService.GetTransactionsByUserId(loggedUser.Id);
            return Ok(new ApiResponse<TransactionDTO[]>
            {
                Error = false,
                Data = transactions,
                Message = "Historial de transacciones obtenido exitosamente"
            });
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

    /// <summary>
    /// Obtiene los productos de una transacción específica
    /// </summary>
    /// <param name="transactionId">ID de la transacción</param>
    /// <returns>Lista de productos de la transacción</returns>
    /// <response code="200">Retorna la lista de productos de la transacción</response>
    /// <response code="400">Si hay un error en la obtención</response>
    /// <response code="403">Si el usuario no tiene permiso para acceder a esta transacción</response>
    /// <response code="404">Si la transacción no existe</response>
    [HttpGet("{transactionId}/items")]
    [ProducesResponseType(typeof(ApiResponse<TransactionItemDTO[]>), 200)]
    public async Task<ActionResult<ApiResponse<TransactionItemDTO[]>>> GetTransactionItems(int transactionId)
    {
        try
        {
            var loggedUser = ObtainUserFromToken();

            // Primero verificamos que la transacción pertenezca al usuario
            var transaction = await TransactionService.GetTransactionById(transactionId);
            if (transaction.UserId != loggedUser.Id)
            {
                return Forbid();
            }

            var items = await TransactionService.GetTransactionItems(transactionId);

            return Ok(new ApiResponse<TransactionItemDTO[]>
            {
                Error = false,
                Data = items,
                Message = "Items de la transacción obtenidos exitosamente"
            });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("No existe una transacción"))
            {
                return NotFound(new ApiResponse<TransactionItemDTO[]>
                {
                    Error = true,
                    Message = ex.Message
                });
            }

            return BadRequest(new ApiResponse<TransactionItemDTO[]>
            {
                Error = true,
                Message = ex.Message
            });
        }
    }
}
