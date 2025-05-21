using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Models.DAO;
using System.Linq.Expressions;

namespace ServiPuntosUy.DataServices.Services.Central;

public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IEstacionService _estacionService;

    public ProductService(IGenericRepository<Product> productRepository, IEstacionService estacionService)
    {
        _productRepository = productRepository;
        _estacionService = estacionService;
    }

    // Métodos de los Productos

    public ProductDTO GetProductDTO(Product product) {
      return new ProductDTO {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        EstacionId = product.EstacionId,
        Estacion = _estacionService.GetEstacionDTO(product.Estacion)
      };
    }

    public ProductDTO GetProductById(int id) {
      var product = _productRepository.GetQueryable().FirstOrDefault(p => p.Id == id);
      return product != null ? GetProductDTO(product) : null;
    }

    public ProductDTO CreateProduct(string name, int price, int estacionId) {
      var estacion = _estacionService.GetEstacionById(estacionId);
      if (estacion == null) {
        throw new ArgumentException("No existe una estación con el id " + estacionId);
      }

      var product = new Product {
        Name = name,
        Price = price,
        EstacionId = estacionId,
      };

      var createdProduct = _productRepository.AddAsync(product).GetAwaiter().GetResult();
      _productRepository.SaveChangesAsync().GetAwaiter().GetResult();

      return GetProductDTO(createdProduct);
    }

    public IEnumerable<ProductDTO> GetProductsByEstacionId(int estacionId) {
      var products = _productRepository.GetQueryable().Where(p => p.EstacionId == estacionId);
      return products.Select(GetProductDTO);
    }
}
