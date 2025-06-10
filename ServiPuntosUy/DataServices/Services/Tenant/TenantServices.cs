using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using ServiPuntosUy.Models.DAO;
using System.Text.RegularExpressions;
using ServiPuntosUy.DAO.Data.Central;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DataServices.Services.Branch;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.CommonLogic;

namespace ServiPuntosUy.DataServices.Services.Tenant
{
    public class TenantBranchService : ITenantBranchService
    {
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly IGenericRepository<DAO.Models.Central.FuelPrices> _fuelPricesRepository;

        public TenantBranchService(
            IGenericRepository<DAO.Models.Central.Branch> branchRepository,
            IGenericRepository<DAO.Models.Central.FuelPrices> fuelPricesRepository)
        {
            _branchRepository = branchRepository;
            _fuelPricesRepository = fuelPricesRepository;
        }
    

        // Métodos de Branch

        public BranchDTO GetBranchDTO(ServiPuntosUy.DAO.Models.Central.Branch branch)
        {
            return new BranchDTO
            {
                Id = branch.Id,
                Address = branch.Address,
                Latitud = branch.Latitud,
                Longitud = branch.Longitud,
                Phone = branch.Phone,
                OpenTime = branch.OpenTime,
                ClosingTime = branch.ClosingTime,
                TenantId = branch.TenantId,
            };
        }

        public BranchDTO CreateBranch(int tenantId, string latitud, string longitud, string address, string phone, TimeOnly openTime, TimeOnly closingTime)
        {
            // Crear un nuevo branch
            var branch = new DAO.Models.Central.Branch
            {
                TenantId = tenantId,
                Latitud = latitud,
                Longitud = longitud,
                Address = address,
                Phone = phone,
                OpenTime = openTime,
                ClosingTime = closingTime,
            };

            // Guardar el branch en la base de datos usando el repositorio de la clase
            var createdBranch = _branchRepository.AddAsync(branch).GetAwaiter().GetResult();
            _branchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Inicializar los precios de combustibles para este branch
            InitializeFuelPrices(createdBranch.Id, tenantId);

            // Devolver el DTO del branch creado
            return GetBranchDTO(createdBranch);
        }

        // Método privado para inicializar los precios de combustibles
        private void InitializeFuelPrices(int branchId, int tenantId)
        {
            // Crear un registro para cada tipo de combustible
            var fuelTypes = Enum.GetValues(typeof(FuelType)).Cast<FuelType>();

            foreach (var fuelType in fuelTypes)
            {
                // Obtener el precio predeterminado del enum FuelPrice
                decimal price = (int)Enum.Parse(typeof(FuelPrice), fuelType.ToString());

                var fuelPrice = new DAO.Models.Central.FuelPrices
                {
                    BranchId = branchId,
                    TenantId = tenantId,
                    FuelType = fuelType,
                    Price = price
                };

                _fuelPricesRepository.AddAsync(fuelPrice).GetAwaiter().GetResult();
            }

            // Guardar todos los cambios
            _fuelPricesRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public BranchDTO UpdateBranch(int branchId, string? latitud, string? longitud, string? address, string? phone, TimeOnly? openTime, TimeOnly? closingTime)
        {
            var branch = _branchRepository.GetByIdAsync(branchId).GetAwaiter().GetResult();
            if (branch == null)
            {
                throw new Exception("No existe una estación con el ID ${branchId}");
            }
            if (latitud != null)
            {
                branch.Latitud = latitud;
            }
            if (longitud != null)
            {
                branch.Longitud = longitud;
            }
            if (address != null)
            {
                branch.Address = address;
            }
            if (phone != null)
            {
                branch.Phone = phone;
            }
            if (openTime != null)
            {
                branch.OpenTime = openTime.Value;
            }
            if (closingTime != null)
            {
                branch.ClosingTime = closingTime.Value;
            }

            _branchRepository.UpdateAsync(branch).GetAwaiter().GetResult();
            _branchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return GetBranchDTO(branch);
        }

        public void DeleteBranch(int branchId)
        {
            var branch = _branchRepository.GetByIdAsync(branchId).GetAwaiter().GetResult();
            if (branch == null)
            {
                throw new Exception("No existe una estación con el ID ${branchId}");
            }

            _branchRepository.DeleteAsync(branchId).GetAwaiter().GetResult();
            _branchRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public BranchDTO setBranchHours(int id,  TimeOnly openTime, TimeOnly closingTime)
        {
            // Obtener la estación por ID
            var branch = _branchRepository.GetByIdAsync(id).GetAwaiter().GetResult();
            if (branch == null)
            {
                throw new Exception($"No existe una estación con el ID {id}");
            }

            // Actualizar las horas de apertura y cierre
            branch.OpenTime = openTime;
            branch.ClosingTime = closingTime;

            // Guardar los cambios en la base de datos
            _branchRepository.UpdateAsync(branch).GetAwaiter().GetResult();
            _branchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return GetBranchDTO(branch);
        }
        
        public BranchDTO[] GetBranchList(int tenantId)
        {
            // Obtener la lista de branches del repositorio filtrando por TenantId
            var branches = _branchRepository.GetQueryable()
                .Where(e => e.TenantId == tenantId).ToList();

            return branches.Select(b => GetBranchDTO(b)).ToArray();

        }
    }

    /// <summary>
    /// Implementación del servicio de lealtad para el administrador de tenant
    /// </summary>
    public class LoyaltyService : ILoyaltyService
    {
        private readonly DbContext _dbContext;
        private readonly IGenericRepository<LoyaltyConfig> _loyaltyConfigRepository;

        public LoyaltyService(DbContext dbContext, IGenericRepository<LoyaltyConfig> loyaltyConfigRepository)
        {
            _dbContext = dbContext;
            _loyaltyConfigRepository = loyaltyConfigRepository;
        }

        /// <summary>
        /// Actualiza un programa de fidelidad para un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="pointsName">Nombre de los puntos</param>
        /// <param name="pointsValue">Valor de los puntos</param>
        /// <param name="accumulationRule">Regla de acumulación de puntos</param>
        /// <param name="expiricyPolicyDays">Días de expiración de los puntos</param>
        /// <returns>Configuración de lealtad actualizada</returns>
        public LoyaltyConfigDTO UpdateLoyaltyProgram(int tenantId, string? pointsName, int? pointsValue, decimal? accumulationRule, int? expiricyPolicyDays)
        {
            var existingLoyaltyConfig = _loyaltyConfigRepository.GetQueryable().FirstOrDefault(lc => lc.TenantId == tenantId);

            if (existingLoyaltyConfig == null) {
                throw new Exception($"No existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            existingLoyaltyConfig.PointsName = pointsName ?? existingLoyaltyConfig.PointsName;

            existingLoyaltyConfig.PointsValue = (pointsValue.HasValue && pointsValue.Value != 0)
                ? pointsValue.Value
                : existingLoyaltyConfig.PointsValue;

            existingLoyaltyConfig.AccumulationRule = accumulationRule ?? existingLoyaltyConfig.AccumulationRule;

            existingLoyaltyConfig.ExpiricyPolicyDays = (expiricyPolicyDays.HasValue && expiricyPolicyDays.Value != 0)
                ? expiricyPolicyDays.Value
                : existingLoyaltyConfig.ExpiricyPolicyDays;

            _loyaltyConfigRepository.UpdateAsync(existingLoyaltyConfig).GetAwaiter().GetResult();
            _loyaltyConfigRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return GetLoyaltyConfigDTO(existingLoyaltyConfig);
        }

        /// <summary>
        /// Crea un programa de fidelidad para un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="pointsName">Nombre de los puntos</param>
        /// <param name="pointsValue">Valor de los puntos</param>
        /// <param name="accumulationRule">Regla de acumulación de puntos</param>
        /// <param name="expiricyPolicyDays">Días de expiración de los puntos</param>
        /// <returns>Configuración de lealtad creado</returns>
        public LoyaltyConfigDTO CreateLoyaltyProgram(int tenantId, string pointsName, int pointsValue, decimal accumulationRule, int expiricyPolicyDays)
        {
            var existingLoyaltyConfig = _loyaltyConfigRepository.GetQueryable().FirstOrDefault(lc => lc.TenantId == tenantId);

            if (existingLoyaltyConfig != null) {
                throw new Exception($"Ya existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            // Crear la configuración de lealtad para el tenant
            var newLoyaltyConfig = new DAO.Models.Central.LoyaltyConfig {
                TenantId = tenantId,
                PointsName = pointsName,
                PointsValue = pointsValue,
                AccumulationRule = accumulationRule,
                ExpiricyPolicyDays = expiricyPolicyDays
            };

            var createdLoyaltyConfig = _loyaltyConfigRepository.AddAsync(newLoyaltyConfig).GetAwaiter().GetResult();
            _loyaltyConfigRepository.SaveChangesAsync().GetAwaiter().GetResult();

            return GetLoyaltyConfigDTO(createdLoyaltyConfig);
        }

        /// <summary>
        /// Verifica si los puntos del usuario han expirado según la política de expiración
        /// y actualiza la fecha del último login
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si los puntos expiraron, False en caso contrario</returns>
        public Task<bool> CheckPointsExpirationAsync(int userId)
        {
            // Para administradores de tenant, no aplicamos la lógica de expiración de puntos
            return Task.FromResult(false);
        }

        /// <summary>
        /// Convierte un modelo de configuración de lealtad a DTO
        /// </summary>
        /// <param name="loyaltyConfig">Modelo de configuración de lealtad</param>
        /// <returns>DTO de configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyConfigDTO(LoyaltyConfig loyaltyConfig) {
            return new LoyaltyConfigDTO {
                Id = loyaltyConfig.Id,
                TenantId = loyaltyConfig.TenantId,
                PointsName = loyaltyConfig.PointsName,
                PointsValue = loyaltyConfig.PointsValue,
                AccumulationRule = loyaltyConfig.AccumulationRule,
                ExpiricyPolicyDays = loyaltyConfig.ExpiricyPolicyDays
            };
        }

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyProgram(int tenantId)
        {
            var loyaltyConfig = _loyaltyConfigRepository.GetQueryable().FirstOrDefault(lc => lc.TenantId == tenantId);

            if (loyaltyConfig == null) {
                throw new ArgumentException($"No existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            return GetLoyaltyConfigDTO(loyaltyConfig);
        }

    }

    /// <summary>
    /// Implementación del servicio de promociones para el administrador de tenant
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly IGenericRepository<DAO.Models.Central.Promotion> _promotionRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionBranch> _promotionBranchRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionProduct> _promotionProductRepository;

        public PromotionService(IGenericRepository<DAO.Models.Central.Promotion> promotionRepository,
                                IGenericRepository<DAO.Models.Central.PromotionProduct> promotionProductRepository,
                                IGenericRepository<DAO.Models.Central.PromotionBranch> promotionBranchRepository)
        
        {
            _promotionBranchRepository = promotionBranchRepository;
            _promotionProductRepository = promotionProductRepository;
            _promotionRepository = promotionRepository;
        }

        // Implementar los métodos de la interfaz IPromotionService
        // Esta es una implementación básica para el scaffold

        public PromotionDTO GetPromotionDTO(DAO.Models.Central.Promotion promotion)
        {
            return new PromotionDTO
            {
                Id = promotion.Id,
                TenantId = promotion.TenantId,
                BranchId = promotion.BranchId,
                Description = promotion.Description,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }


        public Task<PromotionDTO?> AddPromotion(int tenantId, int branchId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product)        
        {
            var promotion = new DAO.Models.Central.Promotion
            {
                TenantId = tenantId,
                Description = description,
                StartDate = startDate,
                EndDate = endDate
            };

            // Guardar la promoción en la base de datos usando el repositorio de la clase
            var createdPromotion = _promotionRepository.AddAsync(promotion).GetAwaiter().GetResult();
            _promotionRepository.SaveChangesAsync().GetAwaiter().GetResult();

            var promotionDTO = GetPromotionDTO(createdPromotion);
            // Asociar los productos a la promoción
            foreach (var productId in product)
            {
                var promotionProduct = new DAO.Models.Central.PromotionProduct
                {
                    PromotionId = promotion.Id,
                    ProductId = productId
                };
                _promotionProductRepository.AddAsync(promotionProduct).GetAwaiter().GetResult();
            }
            _promotionProductRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Asociar los branches a la promoción
            foreach (var bId in branch)
            {
                var promotionBranch = new DAO.Models.Central.PromotionBranch
                {
                    PromotionId = promotion.Id,
                    BranchId = bId
                };
                _promotionBranchRepository.AddAsync(promotionBranch).GetAwaiter().GetResult();
            }
            _promotionBranchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Devolver el DTO de la promoción creada
            return Task.FromResult(promotionDTO);
        }
    //         public Task<PromotionDTO?> AddPromotion(int tenantId, int branchId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product)

    // {
    //     // Crear la promoción (simulación)
    //     var promotion = new DAO.Models.Central.Promotion
    //     {
    //                 TenantId = tenantId,
    //                 Description = description,
    //                 StartDate = startDate,
    //                 EndDate = endDate
    //     };

    //     // Simular guardar la promoción en la base de datos
    //     Console.WriteLine("Promotion Created:");
    //     Console.WriteLine($"Description: {promotion.Description}, StartDate: {promotion.StartDate}, EndDate: {promotion.EndDate}");

    //     // Listas temporales para simular la base de datos
    //     var promotionProducts = new List<DAO.Models.Central.PromotionProduct>();
    //     var promotionBranches = new List<DAO.Models.Central.PromotionBranch>();

    //     // Asociar los productos a la promoción
    //     foreach (var productId in product)
    //     {
    //         var promotionProduct = new DAO.Models.Central.PromotionProduct
    //         {
    //             PromotionId = promotion.Id,
    //             ProductId = productId
    //         };
    //         promotionProducts.Add(promotionProduct); // Agregar a la lista temporal
    //     }

    //     // Imprimir los resultados de promotionProducts
    //     Console.WriteLine("Promotion Products:");
    //     foreach (var promotionProduct in promotionProducts)
    //     {
    //         Console.WriteLine($"PromotionId: {promotionProduct.PromotionId}, ProductId: {promotionProduct.ProductId}");
    //     }

    //     // Asociar los branches a la promoción
    //     foreach (var estacionId in branch)
    //     {
    //         var promotionBranch = new DAO.Models.Central.PromotionBranch
    //         {
    //             PromotionId = promotion.Id,
    //             BranchId = estacionId
    //         };
    //         promotionBranches.Add(promotionBranch); // Agregar a la lista temporal
    //     }

    //     // Imprimir los resultados de promotionBranches
    //     Console.WriteLine("Promotion Branches:");
    //     foreach (var promotionBranch in promotionBranches)
    //     {
    //         Console.WriteLine($"PromotionId: {promotionBranch.PromotionId}, BranchId: {promotionBranch.BranchId}");
    //     }

    //     // Devolver el DTO de la promoción creada
    //     // return Task.FromResult(promotionDTO);
    //     var promotionDTO = GetPromotionDTO(promotion);
    //     return Task.FromResult<PromotionDTO?>(promotionDTO);
    // }
    
    }

    /// <summary>
    /// Implementación del servicio de productos para el administrador de tenant
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<DAO.Models.Central.Product> _productRepository;

        public ProductService(IGenericRepository<DAO.Models.Central.Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public ProductDTO GetProductDTO(DAO.Models.Central.Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                TenantId = product.TenantId,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                AgeRestricted = product.AgeRestricted
            };
        }
        public async Task<ProductDTO?> GetProductById(int productId)
        {
            // var product = await _productRepository.GetByIdAsync(productId)
                var product = await _productRepository.GetQueryable()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Id == productId);

            return product is not null ? GetProductDTO(product) : null;
        }

        public ProductDTO[] GetProductList(int tenantId)
        {
            var products = _productRepository.GetQueryable().Where(product => product.TenantId == tenantId).ToList();
            return [.. products.Select(GetProductDTO)];
        }
        public ProductDTO CreateProduct(int tenantId, string name, string description, string imageUrl, decimal price, bool ageRestricted)
        {
            var product = new DAO.Models.Central.Product
            {
                TenantId = tenantId,
                Name = name,
                Description = description,
                ImageUrl = imageUrl,
                Price = price,
                AgeRestricted = ageRestricted
            };

            // Guardar el producto en la base de datos usando el repositorio de la clase
            var createdProduct = _productRepository.AddAsync(product).GetAwaiter().GetResult();
            _productRepository.SaveChangesAsync().GetAwaiter().GetResult();
            // Devolver el DTO del producto creado

            return GetProductDTO(createdProduct);

        }

                // Método de mapeo
        private DAO.Models.Central.Product MapToProduct(ProductDTO productDTO)
        {
            return new DAO.Models.Central.Product
            {
                Id = productDTO.Id,
                TenantId = productDTO.TenantId,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
            };
        }

        public async Task<ProductDTO?> UpdateProduct(int productId, string? name, string? description, string? imageUrl, decimal? price, bool? ageRestricted)
        {

            var productDTO = await GetProductById(productId);
            if (productDTO == null)
            {
                throw new Exception($"No existe un producto con el ID {productId}");
            }

            var product = MapToProduct(productDTO); //Update no espera DTO

            product.Name = name ?? product.Name;
            product.Description = description ?? product.Description;
            product.ImageUrl = imageUrl ?? product.ImageUrl;
            product.Price = price ?? product.Price;
            product.AgeRestricted = ageRestricted ?? product.AgeRestricted;


            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return GetProductDTO(product);
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var productDTO = await GetProductById(productId);
            if (productDTO == null)
            {
                return false;
            }

            _productRepository.DeleteAsync(productId).GetAwaiter().GetResult();
            _productRepository.SaveChangesAsync().GetAwaiter().GetResult();
            return true;
        }

    }

    /// <summary>
    /// Implementación del servicio de usuarios para el administrador de tenant
    /// </summary>
    public class UserService : IUserService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public UserService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IUserService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de notificaciones para el administrador de tenant
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public NotificationService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz INotificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de verificación para el administrador de tenant
    /// </summary>
    public class VerificationService : IVerificationService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public VerificationService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IVerificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de reportes para el administrador de tenant
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public ReportingService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IReportingService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de pagos para el administrador de tenant
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public PaymentService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IPaymentService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de estadísticas para el administrador de tenant
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public StatisticsService(DbContext dbContext, IConfiguration configuration, ITenantAccessor tenantAccessor)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantAccessor.GetCurrentTenantId();
        }

        /// <summary>
        /// Obtiene las estadísticas para el administrador de tenant
        /// </summary>
        /// <returns>Estadísticas específicas del tenant</returns>
        public async Task<object> GetStatisticsAsync()
        {
            // Convertir tenantId a int para las consultas
            if (!int.TryParse(_tenantId, out int tenantIdInt))
            {
                throw new ArgumentException("TenantId inválido");
            }

            // Contar usuarios por tipo para este tenant
            var usersByType = await _dbContext.Set<DAO.Models.Central.User>()
                .Where(u => u.TenantId == tenantIdInt)
                .GroupBy(u => u.Role)
                .Select(g => new { UserType = g.Key, Count = g.Count() })
                .ToListAsync();

            // Contar el total de usuarios para este tenant
            int totalUsers = await _dbContext.Set<DAO.Models.Central.User>()
                .Where(u => u.TenantId == tenantIdInt)
                .CountAsync();

            // Contar el total de transacciones para este tenant
            int totalTransactions = await _dbContext.Set<DAO.Models.Central.Transaction>()
                .Where(t => t.TenantId == tenantIdInt)
                .CountAsync();

            // Contar promociones por tipo (tenant o branch) para este tenant
            var tenantPromotions = await _dbContext.Set<DAO.Models.Central.Promotion>()
                .Where(p => p.TenantId == tenantIdInt && p.BranchId == null)
                .CountAsync();

            var branchPromotions = await _dbContext.Set<DAO.Models.Central.Promotion>()
                .Where(p => p.TenantId == tenantIdInt && p.BranchId != null)
                .CountAsync();

            int totalPromotions = tenantPromotions + branchPromotions;

            // Construir el objeto de respuesta
            var statistics = new
            {
                users = new
                {
                    total = totalUsers,
                    byType = new
                    {
                        tenant = usersByType.FirstOrDefault(u => u.UserType == UserType.Tenant)?.Count ?? 0,
                        branch = usersByType.FirstOrDefault(u => u.UserType == UserType.Branch)?.Count ?? 0,
                        endUser = usersByType.FirstOrDefault(u => u.UserType == UserType.EndUser)?.Count ?? 0
                    }
                },
                transactions = new
                {
                    total = totalTransactions
                },
                promotions = new
                {
                    total = totalPromotions,
                    tenantPromotions,
                    branchPromotions
                }
            };

            return statistics;
        }
    }
}
