using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services;

public interface IProductService
{
    ProductDTO GetProductDTO(Product product);
    ProductDTO GetProductById(int id);
    ProductDTO CreateProduct(string name, int price, int estacionId);
    IEnumerable<ProductDTO> GetProductsByEstacionId(int estacionId);
}
