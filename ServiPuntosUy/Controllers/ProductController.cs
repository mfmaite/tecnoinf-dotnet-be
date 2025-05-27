using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using Microsoft.AspNetCore.Authorization;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : BaseController
{
    public ProductController(IServiceFactory serviceFactory) : base(serviceFactory)
    {
    }

    /// <summary>
    /// Crear un nuevo branch
    /// </summary>
    /// <param name="product">Datos del branch a crear</param>
    /// <returns>El branch creado</returns>
    /// <response code="200">Retorna el branch creado</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult AddNewProduct([FromBody] AddNewProductRequest request) {
        try {

            if (request == null)
                return BadRequest("Los datos del producto son requeridos.");

            // if (int.Parse(ObtainTenantFromToken()) != request.tenantId)
            //     return BadRequest("No tiene permisos para crear productos en este tenant.");

            // if (ObtainUserTypeFromToken() != UserType.Tenant)
            //     return BadRequest("No tiene permisos para crear productos.");

            // Validar que ningún campo requerido esté vacío
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Description) 
                || string.IsNullOrWhiteSpace(request.ImageUrl) || request.Price <= 0)
                return BadRequest("Verifica que todos los campos requeridos estén completos: Name, Description, ImageUrl y Price.");

            var newProduct = ProductService?.CreateProduct(
                request.tenantId,
                request.Name,
                request.Description,
                request.ImageUrl,
                request.Price,
                request.AgeRestricted
            );

            if (newProduct == null)
            {
                return BadRequest(new ApiResponse<object>{
                    Error = true,
                    Message = "No se pudo crear el producto. El servicio no está disponible."
                });
            }
            return Ok(new ApiResponse<ProductDTO>{
                Error = false,
                Message = "Producto creado correctamente",
                Data = newProduct
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }


}
