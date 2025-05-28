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
        ProductDTO UpdateProduct(int productId, string? name, string? description, string? imageUrl, decimal? price, bool? ageRestricted);
        void DeleteProduct(int productId);
    
    }
}
