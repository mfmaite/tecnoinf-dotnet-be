using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación del servicio de aplicación de promociones
    /// </summary>
    public class PromotionApplicator : IPromotionApplicator
    {
        private readonly IGenericRepository<Promotion> _promotionRepository;
        private readonly IGenericRepository<PromotionProduct> _promotionProductRepository;
        private readonly IGenericRepository<PromotionBranch> _promotionBranchRepository;
        
        public PromotionApplicator(
            IGenericRepository<Promotion> promotionRepository,
            IGenericRepository<PromotionProduct> promotionProductRepository,
            IGenericRepository<PromotionBranch> promotionBranchRepository)
        {
            _promotionRepository = promotionRepository;
            _promotionProductRepository = promotionProductRepository;
            _promotionBranchRepository = promotionBranchRepository;
        }
        
        /// <summary>
        /// Obtiene la promoción aplicable para un producto en una sucursal específica
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Promoción aplicable, o null si no hay promoción</returns>
        public async Task<Promotion> GetApplicablePromotion(int productId, int branchId, int tenantId)
        {
            // Obtener fecha actual
            var now = DateTime.UtcNow;
            
            // Buscar promociones activas para el producto en la branch específica
            var branchPromotions = await GetBranchSpecificPromotions(productId, branchId, tenantId, now);
            
            // Si hay promociones específicas para la branch, devolver la más ventajosa
            if (branchPromotions.Any())
            {
                return branchPromotions.OrderBy(p => p.Price).FirstOrDefault();
            }
            
            // Si no hay promociones específicas para la branch, buscar promociones a nivel de tenant
            var tenantPromotions = await GetTenantWidePromotions(productId, tenantId, now);
            
            return tenantPromotions.OrderBy(p => p.Price).FirstOrDefault();
        }
        
        /// <summary>
        /// Obtiene el precio promocional para un producto en una sucursal específica
        /// </summary>
        /// <param name="productId">ID del producto</param>
        /// <param name="branchId">ID de la sucursal</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="originalPrice">Precio original del producto</param>
        /// <returns>Precio promocional si hay una promoción aplicable, o el precio original si no hay promoción</returns>
        public async Task<decimal> GetPromotionalPrice(int productId, int branchId, int tenantId, decimal originalPrice)
        {
            var promotion = await GetApplicablePromotion(productId, branchId, tenantId);
            
            if (promotion != null && promotion.Price > 0)
            {
                // Si hay un precio promocional, usarlo
                return promotion.Price;
            }
            
            // Si no hay promoción aplicable, devolver el precio original
            return originalPrice;
        }
        
        /// <summary>
        /// Obtiene las promociones específicas para una branch
        /// </summary>
        private async Task<List<Promotion>> GetBranchSpecificPromotions(int productId, int branchId, int tenantId, DateTime now)
        {
            // Obtener IDs de promociones que aplican a esta branch
            var promotionIds = await _promotionBranchRepository.GetQueryable()
                .Where(pb => pb.BranchId == branchId && pb.TenantId == tenantId)
                .Select(pb => pb.PromotionId)
                .ToListAsync();
                
            if (!promotionIds.Any())
            {
                return new List<Promotion>();
            }
            
            // Obtener IDs de promociones que aplican a este producto
            var productPromotionIds = await _promotionProductRepository.GetQueryable()
                .Where(pp => pp.ProductId == productId)
                .Select(pp => pp.PromotionId)
                .ToListAsync();
                
            if (!productPromotionIds.Any())
            {
                return new List<Promotion>();
            }
            
            // Intersección de promociones que aplican tanto a la branch como al producto
            var applicablePromotionIds = promotionIds.Intersect(productPromotionIds).ToList();
            
            if (!applicablePromotionIds.Any())
            {
                return new List<Promotion>();
            }
            
            // Obtener las promociones activas
            return await _promotionRepository.GetQueryable()
                .Where(p => applicablePromotionIds.Contains(p.Id) && 
                       p.TenantId == tenantId && 
                       p.StartDate <= now && 
                       p.EndDate >= now)
                .ToListAsync();
        }
        
        /// <summary>
        /// Obtiene las promociones a nivel de tenant (que aplican a todas las branches)
        /// </summary>
        private async Task<List<Promotion>> GetTenantWidePromotions(int productId, int tenantId, DateTime now)
        {
            // Obtener IDs de promociones que aplican a este producto
            var productPromotionIds = await _promotionProductRepository.GetQueryable()
                .Where(pp => pp.ProductId == productId)
                .Select(pp => pp.PromotionId)
                .ToListAsync();
                
            if (!productPromotionIds.Any())
            {
                return new List<Promotion>();
            }
            
            // Obtener IDs de todas las promociones
            var allPromotionIds = await _promotionRepository.GetQueryable()
                .Where(p => p.TenantId == tenantId)
                .Select(p => p.Id)
                .ToListAsync();
                
            // Obtener IDs de promociones que están asociadas a alguna branch
            var branchPromotionIds = await _promotionBranchRepository.GetQueryable()
                .Where(pb => pb.TenantId == tenantId)
                .Select(pb => pb.PromotionId)
                .Distinct()
                .ToListAsync();
                
            // Promociones que no están asociadas a ninguna branch específica (aplican a nivel de tenant)
            var tenantWidePromotionIds = allPromotionIds.Except(branchPromotionIds).ToList();
            
            // Intersección con promociones que aplican al producto
            var applicablePromotionIds = tenantWidePromotionIds.Intersect(productPromotionIds).ToList();
            
            if (!applicablePromotionIds.Any())
            {
                return new List<Promotion>();
            }
            
            // Obtener las promociones activas
            return await _promotionRepository.GetQueryable()
                .Where(p => applicablePromotionIds.Contains(p.Id) && 
                       p.TenantId == tenantId && 
                       p.StartDate <= now && 
                       p.EndDate >= now)
                .ToListAsync();
        }
    }
}
