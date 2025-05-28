using ServiPuntosUy.DTO;
using ServiPuntosUy.DataServices.Services.Tenant;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de productos
    /// </summary>
    public interface IProductService
    {
        ProductDTO CreateProduct(int tenantId, string name, string description, string imageUrl, decimal price, bool ageRestricted);
        Task<ProductDTO?> GetProductById(int productId);
        ProductDTO[] GetProductList(int tenantId);

    }
}
