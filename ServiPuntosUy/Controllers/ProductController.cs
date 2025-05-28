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

            // if (int.Parse(ObtainTenantFromToken()) != request.tenantId)
            //     return BadRequest("No tiene permisos para crear productos en este producto.");

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
                return BadRequest(new ApiResponse<object>
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

}
