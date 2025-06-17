using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUy.DataServices.Services;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FuelController : BaseController
{
    public FuelController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Obtiene todos los precios de combustible para una estación
    /// </summary>
    /// <param name="branchId">ID de la estación</param>
    /// <returns>Lista de precios de combustible</returns>
    [HttpGet("{branchId}/prices")]
    [ProducesResponseType(typeof(List<FuelPrices>), 200)]
    [ProducesResponseType(400)]
    public IActionResult GetAllFuelPrices(int branchId)
    {
        try
        {
            var fuelPrices = FuelService.GetAllFuelPrices(branchId);
            
            if (fuelPrices == null || !fuelPrices.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = $"No se encontraron precios de combustible para la estación {branchId}"
                });
            }

            return Ok(new ApiResponse<List<FuelPrices>>
            {
                Error = false,
                Message = "Precios obtenidos correctamente",
                Data = fuelPrices
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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
    /// Obtiene el precio de un combustible para una estación
    /// </summary>
    /// <param name="branchId">ID de la estación</param>
    /// <param name="fuelType">Tipo de combustible</param>
    /// <returns>El precio del combustible</returns>
    [HttpGet("{branchId}/price/{fuelType}")]
    [ProducesResponseType(typeof(FuelPrices), 200)]
    [ProducesResponseType(400)]
    public IActionResult GetFuelPrice(int branchId, FuelType fuelType)
    {
        try
        {
            var fuelPrice = FuelService.GetFuelPrice(branchId, fuelType);
            
            if (fuelPrice == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = $"No se encontró precio para el combustible {fuelType} en la estación {branchId}"
                });
            }

            return Ok(new ApiResponse<FuelPrices>
            {
                Error = false,
                Message = "Precio obtenido correctamente",
                Data = fuelPrice
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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
    /// Actualiza el precio de un combustible para una estación
    /// </summary>
    /// <param name="branchId">ID de la estación</param>
    /// <param name="fuelType">Tipo de combustible</param>
    /// <param name="price">Nuevo precio</param>
    /// <returns>El precio actualizado</returns>
    [HttpPut("{branchId}/price/{fuelType}")]
    [ProducesResponseType(typeof(FuelPrices), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public IActionResult UpdateFuelPrice(int branchId, FuelType fuelType, [FromBody] decimal price)
    {
        try
        {
            var updatedPrice = FuelService.UpdateFuelPrice(branchId, fuelType, price);

            return Ok(new ApiResponse<FuelPrices>
            {
                Error = false,
                Message = "Precio actualizado correctamente",
                Data = updatedPrice
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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
