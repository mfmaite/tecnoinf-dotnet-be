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
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public LoyaltyService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        public Task<LoyaltyConfigDTO> GetLoyaltyConfigAsync(string tenantId)
        {
            // Un administrador de tenant solo puede obtener información de su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Actualiza la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="config">Configuración de lealtad</param>
        /// <returns>Configuración de lealtad actualizada</returns>
        public Task<LoyaltyConfigDTO> UpdateLoyaltyConfigAsync(LoyaltyConfigDTO config)
        {
            // Un administrador de tenant solo puede actualizar la configuración de su propio tenant
            if (config.TenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene el saldo de puntos de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Saldo de puntos</returns>
        public Task<int> GetPointsBalanceAsync(int userId, string tenantId)
        {
            // Un administrador de tenant solo puede obtener información de su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registra una transacción de lealtad
        /// </summary>
        /// <param name="transaction">Datos de la transacción</param>
        /// <returns>Transacción registrada</returns>
        public Task<LoyaltyTransactionDTO> RegisterTransactionAsync(LoyaltyTransactionDTO transaction)
        {
            // Un administrador de tenant solo puede registrar transacciones en su propio tenant
            if (transaction.TenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene el historial de transacciones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <param name="page">Número de página</param>
        /// <param name="pageSize">Tamaño de página</param>
        /// <returns>Historial de transacciones</returns>
        public Task<IEnumerable<LoyaltyTransactionDTO>> GetTransactionHistoryAsync(int userId, string tenantId, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            // Un administrador de tenant solo puede obtener información de su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Canjea puntos por un producto o servicio
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="points">Puntos a canjear</param>
        /// <param name="productId">ID del producto (opcional)</param>
        /// <returns>ID de la redención</returns>
        public Task<int> RedeemPointsAsync(int userId, string tenantId, int points, int? productId = null)
        {
            // Un administrador de tenant solo puede canjear puntos en su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica un código QR de redención
        /// </summary>
        /// <param name="qrCode">Código QR</param>
        /// <param name="branchId">ID de la estación</param>
        /// <returns>ID de la redención</returns>
        public Task<int> VerifyRedemptionQrAsync(string qrCode, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Confirma una redención
        /// </summary>
        /// <param name="redemptionId">ID de la redención</param>
        /// <param name="branchId">ID de la estación</param>
        /// <returns>True si la confirmación es exitosa, false en caso contrario</returns>
        public Task<bool> ConfirmRedemptionAsync(int redemptionId, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calcula los puntos a otorgar por una compra
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="amount">Monto de la compra</param>
        /// <param name="productCategory">Categoría del producto</param>
        /// <returns>Puntos a otorgar</returns>
        public Task<int> CalculatePointsAsync(string tenantId, decimal amount, string productCategory)
        {
            // Un administrador de tenant solo puede calcular puntos en su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene estadísticas de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Estadísticas de lealtad</returns>
        public Task<object> GetLoyaltyStatsAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            // Un administrador de tenant solo puede obtener estadísticas de su propio tenant
            if (tenantId != _tenantId)
            {
                throw new UnauthorizedAccessException("No tiene permisos para acceder a este tenant");
            }

            // Implementación básica para el scaffold
            throw new NotImplementedException();
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
    }

    /// <summary>
    /// Implementación del servicio de promociones para el administrador de tenant
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public PromotionService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IPromotionService
        // Esta es una implementación básica para el scaffold
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
}
