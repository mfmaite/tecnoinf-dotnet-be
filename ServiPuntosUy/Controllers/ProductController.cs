using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.Controllers;

public class CreateProductRequest {
  public string Name { get; set; }
  public int Price { get; set; }
  public int EstacionId { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase {
  private readonly IProductService _productService;

  public ProductController(IProductService productService) {
    _productService = productService;
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
  public IActionResult CreateProduct([FromBody] CreateProductRequest request) {
    try {
      var newProduct = _productService.CreateProduct(request.Name, request.Price, request.EstacionId);
      return Ok(newProduct);
    } catch (Exception ex) {
      return BadRequest(ex.Message);
    }
  }


  /// <summary>
  /// Obtener todos los productos de una estación
  /// </summary>
  /// <param name="estacionId">Id de la estación</param>
  /// <returns>Todos los productos de la estación</returns>
  /// <response code="200">Retorna todos los productos de la estación</response>
  /// <response code="400">Si hay un error en la obtención</response>
  [HttpGet("{estacionId}")]
  [ProducesResponseType(typeof(ProductDTO), 200)]
  [ProducesResponseType(400)]
  public IActionResult GetProductsByEstacionId(int estacionId) {
    try {
      var products = _productService.GetProductsByEstacionId(estacionId);

      if (products == null || !products.Any()) {
          return NotFound($"No se encontraron productos para la estación con ID {estacionId}.");
      }

      return Ok(products);
    } catch (Exception ex) {
      return BadRequest(ex.Message);
    }
  }

}
