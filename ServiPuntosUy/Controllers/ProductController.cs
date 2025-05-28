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
    /// Crear un nuevo producto
    /// </summary>
    /// <param name="product">Datos del producto a crear</param>
    /// <returns>El producto creado</returns>
    /// <response code="200">Retorna el producto creado</response>
    /// <response code="400">Si hay un error en la creación</response>
    [HttpPost("Create")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult AddNewProduct([FromBody] AddNewProductRequest request) {
        try {

            if (request == null)
                return BadRequest("Los datos del producto son requeridos.");

            if (ObtainUserTypeFromToken() != UserType.Tenant)
                return BadRequest("No tiene permisos para crear productos.");

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

    [HttpPost("Update")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult UpdateProduct([FromBody] AddNewProductRequest request) {
        try {

            if (request == null)
                return BadRequest("Los datos del producto son requeridos.");

            if (ObtainUserTypeFromToken() != UserType.Tenant)
                return BadRequest("No tiene permisos para editar productos.");

            var product = ProductService?.UpdateProduct(
                request.productId,
                request.Name,
                request.Description,
                request.ImageUrl,
                request.Price,
                request.AgeRestricted
            );

            if (product == null)
            {
                return BadRequest(new ApiResponse<object>{
                    Error = true,
                    Message = "No se pudo editar el producto. El servicio no está disponible."
                });
            }
            return Ok(new ApiResponse<ProductDTO>{
                Error = false,
                Message = "Producto editado correctamente",
                Data = product
            });
        }
        catch (Exception ex) {
            return BadRequest(new ApiResponse<object>{
                Error = true,
                Message = ex.Message
            });
        }
    }

    [HttpPost("Update")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    public IActionResult DeleteProduct([FromBody] AddNewProductRequest request) {
        try {

            if (request == null)
                return BadRequest("El id del producto es requerido");

            if (ObtainUserTypeFromToken() != UserType.Tenant)
                return BadRequest("No tiene permisos para eliminar productos.");

            ProductService?.DeleteProduct(request.productId);

            return Ok(new ApiResponse<ProductDTO>{
                Error = false,
                Message = "Producto eliminado correctamente",
                Data = new ProductDTO()
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
