using ServiPuntosUy.DTO;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.Requests;
using ServiPuntosUy.DataServices.Services;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RedemptionController(IServiceFactory serviceFactory) : BaseController(serviceFactory)
{
    private readonly IRedemptionService _redemptionService = serviceFactory.GetService<IRedemptionService>();
    /// <summary>
    /// Genera un token para canjear un producto por puntos
    /// </summary>
    /// <param name="request">Datos del canje</param>
    /// <returns>Token para el canje</returns>
    /// <response code="200">Retorna el token generado</response>
    /// <response code="400">Si hay un error en la generación del token</response>
    [HttpPost("generate-token")]
    [ProducesResponseType(typeof(RedemptionTokenResponse), 200)]
    public async Task<ActionResult<RedemptionTokenResponse>> GenerateRedemptionToken([FromBody] GenerateRedemptionTokenRequest request)
    {
        try
        {
            var loggedUser = ObtainUserFromToken();

            var token = await _redemptionService.GenerateRedemptionToken(
                loggedUser.Id,
                request.BranchId,
                request.ProductId
            );

            return Ok(new ApiResponse<RedemptionTokenResponse>
            {
                Error = false,
                Data = new RedemptionTokenResponse { Token = token },
                Message = "QR generado satisfactoriamente"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<RedemptionTokenResponse>
            {
                Error = true,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Procesa un canje a partir de un token
    /// </summary>
    /// <param name="token">Token JWT con la información del canje</param>
    /// <returns>Página HTML con el resultado del canje</returns>
    /// <response code="200">Retorna una página HTML con el resultado del canje</response>
    [HttpGet("process/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult> ProcessRedemption(string token)
    {
        try
        {
            var transaction = await _redemptionService.ProcessRedemption(token);

            // Devolver una página HTML simple con el resultado
            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Canje Exitoso</title>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; padding: 20px; }}
                    .success {{ color: green; }}
                    .details {{ margin-top: 20px; }}
                </style>
            </head>
            <body>
                <h1 class='success'>¡Canje Exitoso!</h1>
                <div class='details'>
                    <p>Se ha canjeado correctamente el producto.</p>
                    <p>Puntos utilizados: {transaction.PointsSpent}</p>
                    <p>Fecha: {transaction.CreatedAt}</p>
                </div>
            </body>
            </html>";

            return Content(html, "text/html");
        }
        catch (Exception ex)
        {
            var errorHtml = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Error en el Canje</title>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <style>
                    body {{ font-family: Arial, sans-serif; text-align: center; padding: 20px; }}
                    .error {{ color: red; }}
                    .details {{ margin-top: 20px; }}
                </style>
            </head>
            <body>
                <h1 class='error'>Error en el Canje</h1>
                <div class='details'>
                    <p>{ex.Message}</p>
                </div>
            </body>
            </html>";

            return Content(errorHtml, "text/html");
        }
    }
}
