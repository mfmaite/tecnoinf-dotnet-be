using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DataServices.Services.Branch
{
    /// <summary>
    /// Implementación del servicio de precios de combustible para el administrador de branch
    /// </summary>
    public class FuelService : IFuelService
    {
        private readonly IGenericRepository<FuelPrices> _fuelPricesRepository;
        private readonly int _branchId;

        public FuelService(IGenericRepository<FuelPrices> fuelPricesRepository, int branchId)
        {
            _fuelPricesRepository = fuelPricesRepository;
            _branchId = branchId;
        }

        public FuelPrices UpdateFuelPrice(int branchId, FuelType fuelType, decimal price)
        {
            // Verificar que el usuario solo pueda actualizar precios de su propia estación
            if (branchId != _branchId)
            {
                throw new UnauthorizedAccessException("Solo puede actualizar precios de su propia estación");
            }

            // Buscar si ya existe un precio para este combustible en esta estación
            var existingPrice = _fuelPricesRepository.GetQueryable()
                .FirstOrDefault(fp => fp.BranchId == branchId && fp.FuelType == fuelType);

            if (existingPrice != null)
            {
                // Actualizar el precio existente
                existingPrice.Price = price;
                _fuelPricesRepository.UpdateAsync(existingPrice).GetAwaiter().GetResult();
                _fuelPricesRepository.SaveChangesAsync().GetAwaiter().GetResult();
                return existingPrice;
            }
            else
            {
                // No crear un nuevo registro, lanzar excepción
                throw new Exception($"No existe un precio configurado para el combustible {fuelType} en la estación {branchId}");
            }
        }

        public FuelPrices GetFuelPrice(int branchId, FuelType fuelType)
        {
            var fuelPrice = _fuelPricesRepository.GetQueryable()
                .FirstOrDefault(fp => fp.BranchId == branchId && fp.FuelType == fuelType) ?? throw new Exception($"No existe un precio configurado para el combustible {fuelType} en la estación {branchId}");
            return fuelPrice;
        }
    }

    /// <summary>
    /// Implementación del servicio de branches para el administrador de branch
    /// </summary>
    public class BranchService : IBranchService
    {
        private readonly IGenericRepository<ServiPuntosUy.DAO.Models.Central.Branch> _branchRepository;
        private readonly IGenericRepository<ServiPuntosUy.DAO.Models.Central.ProductStock> _productStockRepository;
        private readonly IGenericRepository<ServiPuntosUy.DAO.Models.Central.Product> _productRepository;


        public BranchService(IGenericRepository<ServiPuntosUy.DAO.Models.Central.Branch> branchRepository, IGenericRepository<ServiPuntosUy.DAO.Models.Central.ProductStock> productStockRepository, IGenericRepository<ServiPuntosUy.DAO.Models.Central.Product> productRepository)
        {
            _branchRepository = branchRepository;
            _productStockRepository = productStockRepository;
            _productRepository = productRepository;
        }
        // Métodos del Branch

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

        public async Task<BranchDTO?> GetBranchById(int id)
        {
            // Buscar el branch por ID usando el repositorio de la clase
            var branch = await _branchRepository.GetQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

            // Devolver el DTO si se encontró el branch
            return branch != null ? GetBranchDTO(branch) : null;
        }


        public ProductStockDTO GetProductStockDTO(DAO.Models.Central.ProductStock productStock)
        {
            return new ProductStockDTO
            {
                Id = productStock.Id,
                ProductId = productStock.ProductId,
                BranchId = productStock.BranchId,
                Stock = productStock.Stock,
            };
        }

        public async Task<ProductStockDTO?> GetProductStockById(int productId, int branchId)
        {
            // var product = await _productRepository.GetByIdAsync(productId)
                var productStock = await _productStockRepository.GetQueryable()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Id == productId && p.BranchId == branchId);

            return productStock is not null ? GetProductStockDTO(productStock) : null;
        }

         // Método de mapeo
        private DAO.Models.Central.ProductStock MapToProductStock(ProductStockDTO productStockDTO)
        {
            return new DAO.Models.Central.ProductStock
            {
                Id = productStockDTO.Id,
                ProductId = productStockDTO.ProductId,
                BranchId = productStockDTO.BranchId,
                Stock = productStockDTO.Stock
            };
        }
        public async Task<bool> VerifyProductById(int productId)
        {
            // var product = await _productRepository.GetByIdAsync(productId)
                var product = await _productRepository.GetQueryable()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.Id == productId);

            return product is not null ? true : false;
        }

        public  async Task<ProductStockDTO?> manageStock(int productId, int branchId, int stock)
        {
            try{
                // verificar que existe el producto
                var product = await VerifyProductById(productId);
                if (!product)
                    return null;
                
                var productStockDTO = await GetProductStockById(productId, branchId);
                if (productStockDTO == null) // todavia no se le ha cargado stock
                {
                    productStockDTO = new ProductStockDTO
                    {
                        ProductId = productId,
                        BranchId = branchId,
                        Stock = stock
                    };
                    var stockDTO = MapToProductStock(productStockDTO);
                    await _productStockRepository.AddAsync(stockDTO);
                    await _productStockRepository.SaveChangesAsync();
                    return GetProductStockDTO(stockDTO);  
                }else{
                    // Si ya existe, actualizamos el stock
                    var stockDTO = MapToProductStock(productStockDTO); //Update no espera DTO 
                    stockDTO.Id = productId;
                    stockDTO.Stock = stock;
                    stockDTO.BranchId = branchId; 
                    stockDTO.ProductId = productId; 
                    await _productStockRepository.UpdateAsync(stockDTO);
                    await _productStockRepository.SaveChangesAsync();
                    return GetProductStockDTO(stockDTO);  
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el stock: {ex.Message}");
                return null;
            }   
        }

        public DAO.Models.Central.Branch MapToBranch(BranchDTO branchDTO)
        {
            return new DAO.Models.Central.Branch
            {
                Id = branchDTO.Id,
                Address = branchDTO.Address,
                Latitud = branchDTO.Latitud,
                Longitud = branchDTO.Longitud,
                Phone = branchDTO.Phone,
                OpenTime = branchDTO.OpenTime,
                ClosingTime = branchDTO.ClosingTime,
                TenantId = branchDTO.TenantId
            };
        }

        public async Task<BranchDTO?> setBranchHours(int id,  TimeOnly openTime, TimeOnly closingTime)
        {
            // Obtener la estación por ID
            var branch = await GetBranchById(id);
            if (branch == null)
            {
                throw new Exception($"No existe una estación con el ID {id}");
            }

            // Actualizar las horas de apertura y cierre
            var branchDTO = MapToBranch(branch);
            branchDTO.OpenTime = openTime;
            branchDTO.ClosingTime = closingTime;

            // Guardar los cambios en la base de datos
            await _branchRepository.UpdateAsync(branchDTO);
            await _branchRepository.SaveChangesAsync();

            return GetBranchDTO(branchDTO);
        }
    }

    /// <summary>
    /// Implementación del servicio de lealtad para el administrador de branch
    /// </summary>
    public class LoyaltyService : ILoyaltyService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public LoyaltyService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        public Task<LoyaltyConfigDTO> GetLoyaltyConfigAsync(string tenantId)
        {
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
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifica un código QR de redención
        /// </summary>
        /// <param name="qrCode">Código QR</param>
        /// <param name="branchId">ID del branch</param>
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
        /// <param name="branchId">ID del branch</param>
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
            // Para administradores de branch, no aplicamos la lógica de expiración de puntos
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Implementación del servicio de promociones para el administrador de branch
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public PromotionService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz IPromotionService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de productos para el administrador de branch
    /// </summary>
    // public class ProductService : IProductService

    /// <summary>
    /// Implementación del servicio de usuarios para el administrador de branch
    /// </summary>
    public class UserService : IUserService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public UserService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz IUserService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de notificaciones para el administrador de branch
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public NotificationService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz INotificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de verificación para el administrador de branch
    /// </summary>
    public class VerificationService : IVerificationService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public VerificationService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz IVerificationService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de reportes para el administrador de branch
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public ReportingService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz IReportingService
        // Esta es una implementación básica para el scaffold
    }

    /// <summary>
    /// Implementación del servicio de pagos para el administrador de branch
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;

        public PaymentService(DbContext dbContext, IConfiguration configuration, string tenantId, int branchId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
            _branchId = branchId;
        }

        // Implementar los métodos de la interfaz IPaymentService
        // Esta es una implementación básica para el scaffold
    }
}
