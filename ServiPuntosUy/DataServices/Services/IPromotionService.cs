using ServiPuntosUy.DTO;
using ServiPuntosUy.DataServices.Services.Tenant;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de promociones
    /// </summary>
    public interface IPromotionService
    {
        Task<PromotionDTO?> AddPromotion(int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product);
        Task<PromotionDTO?> UpdatePromotion(int PromotionId,int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product);
        PromotionExtendedDTO[] GetPromotionList(int tenantId);

    }
}
