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
    public IActionResult AddNewProduct([FromBody] AddNewProductRequest request)
    {
        try
        {
            if (request == null)
                return BadRequest("Los datos del producto son requeridos.");

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
                return StatusCode(500, new ApiResponse<object>
                {
                    Error = true,
                    Message = "No se pudo crear el producto. El servicio no está disponible."
                });
            }
            return Ok(new ApiResponse<ProductDTO>
            {
                Error = false,
                Message = "Producto creado correctamente",
                Data = newProduct
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
    /// Obtiene la lista de productos
    /// </summary>
    /// <returns>La lista de productos</returns>
    /// <response code="200">Retorna la lista de productos</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ProductDTO[]), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public IActionResult GetProductList()
    {
        try
        {
            var user = ObtainUserFromToken();
            if (!int.TryParse(user.TenantId, out int tenantId))
            {
                return BadRequest("No se pudo obtener el TenantId del usuario");
            }
            var products = ProductService.GetProductList(tenantId);

            return Ok(new ApiResponse<ProductDTO[]>
            {
                Error = false,
                Message = "Lista de productos obtenida correctamente",
                Data = products
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
    /// Obtiene un producto por su ID
    /// </summary>
    /// <param name="id">ID del producto</param>
    /// <returns>El producto solicitado</returns>
    /// <response code="200">Retorna el producto solicitado</response>
    /// <response code="404">Si el producto no existe</response>
    /// <response code="400">Si hay un error en la búsqueda</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var product = await ProductService.GetProductById(id);
            if (product == null)
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = $"No existe un producto con el ID {id}"
                });

            return Ok(new ApiResponse<ProductDTO>
            {
                Error = false,
                Message = "Producto encontrado correctamente",
                Data = product
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
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateProduct([FromBody] AddNewProductRequest request) {
        try {

            if (request == null)
                return BadRequest("Los datos del producto son requeridos.");

            var product = await ProductService?.UpdateProduct(
                request.productId,
                request.Name,
                request.Description,
                request.ImageUrl,
                request.Price,
                request.AgeRestricted
            );

            if (product == null)
            {
                return StatusCode(500, new ApiResponse<object>{
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

    [HttpPost("Delete")]
    [ProducesResponseType(typeof(ProductDTO), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> DeleteProduct([FromBody] AddNewProductRequest request) {
        try {

            // if (request == null)
            //     return BadRequest("El id del producto es requerido");

            // if (ObtainUserTypeFromToken() != UserType.Tenant)
            //     return BadRequest("No tiene permisos para eliminar productos.");

           var deleteProduct =  await ProductService?.DeleteProduct(request.productId);
           if (!deleteProduct)
            {
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = $"No existe un producto con el ID {request.productId}"
                });
            }

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
