using System;
using ServiceReference;
using System.ServiceModel;
using ServiPuntosUy.Models.DAO;
using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ServiPuntosUy.DataServices.Services.EndUser
{
    /// <summary>
    /// Implementación del servicio de precios de combustible para el usuario final
    /// </summary>
    public class FuelService : IFuelService
    {
        private readonly IGenericRepository<FuelPrices> _fuelPricesRepository;

        public FuelService(IGenericRepository<FuelPrices> fuelPricesRepository)
        {
            _fuelPricesRepository = fuelPricesRepository;
        }

        public FuelPrices UpdateFuelPrice(int branchId, FuelType fuelType, decimal price)
        {
            // El usuario final no puede actualizar precios
            throw new UnauthorizedAccessException("El usuario final no puede actualizar precios de combustible");
        }

        public FuelPrices GetFuelPrice(int branchId, FuelType fuelType)
        {
            var fuelPrice = _fuelPricesRepository.GetQueryable()
                .FirstOrDefault(fp => fp.BranchId == branchId && fp.FuelType == fuelType) ?? throw new Exception($"No existe un precio configurado para el combustible {fuelType} en la estación {branchId}");
            return fuelPrice;
        }

        public List<FuelPrices> GetAllFuelPrices(int branchId)
        {
            // Obtener todos los precios de combustible para la estación
            var fuelPrices = _fuelPricesRepository.GetQueryable()
                .Where(fp => fp.BranchId == branchId)
                .ToList();

            if (fuelPrices.Count == 0)
            {
                throw new Exception($"No existen precios configurados para la estación {branchId}");
            }

            return fuelPrices;
        }
    }

    /// <summary>
    /// Implementación del servicio de lealtad para el usuario final
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

         public LoyaltyConfigDTO CreateLoyaltyProgram(int tenantId, string pointsName, int pointsValue, decimal accumulationRule, int expiricyPolicyDays) {
            throw new UnauthorizedAccessException("El usuario final no puede crear un programa de fidelidad");
        }

        /// <summary>
        /// Verifica si los puntos del usuario han expirado según la política de expiración
        /// y actualiza la fecha del último login
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si los puntos expiraron, False en caso contrario</returns>
        public async Task<bool> CheckPointsExpirationAsync(int userId)
        {
            try
            {
                // Obtener el usuario con AsTracking para asegurar que Entity Framework haga seguimiento de los cambios
                var user = await _dbContext.Set<User>()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return false;

                bool pointsExpired = false;

                // Verificar si el usuario tiene un tenant asignado
                if (user.TenantId.HasValue)
                {
                    // Obtener la configuración de lealtad del tenant
                    var loyaltyConfig = await _dbContext.Set<LoyaltyConfig>()
                        .FirstOrDefaultAsync(lc => lc.TenantId == user.TenantId.Value);

                    if (loyaltyConfig != null)
                    {
                        // Verificar si es la primera vez que inicia sesión o si la fecha es muy antigua (valor por defecto)
                        bool isFirstLogin = user.LastLoginDate.Year < 2000;

                        if (!isFirstLogin)
                        {
                            // Calcular días desde el último login
                            var daysSinceLastLogin = (DateTime.UtcNow - user.LastLoginDate).TotalDays;

                            // Verificar si han pasado más días que los permitidos por la política
                            if (daysSinceLastLogin > loyaltyConfig.ExpiricyPolicyDays)
                            {
                                // Si han pasado más días, los puntos expiran
                                int previousPoints = user.PointBalance;
                                user.PointBalance = 0;
                                pointsExpired = true;

                                // Logging (opcional)
                                Console.WriteLine($"User {user.Id} points expired: {previousPoints} -> 0");
                            }
                        }
                    }
                }

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();

                return pointsExpired;
            }
            catch (Exception ex)
            {
                // Logging del error
                Console.WriteLine($"Error checking point expiration: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Convierte un modelo de configuración de lealtad a DTO
        /// </summary>
        /// <param name="loyaltyConfig">Modelo de configuración de lealtad</param>
        /// <returns>DTO de configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyConfigDTO(DAO.Models.Central.LoyaltyConfig loyaltyConfig) {
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

        public LoyaltyConfigDTO UpdateLoyaltyProgram(int tenantId, string? pointsName, int? pointsValue, decimal? accumulationRule, int? expiricyPolicyDays) {
            throw new UnauthorizedAccessException("El usuario final no puede actualizar un programa de fidelidad");
        }
    }

    /// <summary>
    /// Implementación del servicio de promociones para el usuario final
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly IGenericRepository<DAO.Models.Central.Promotion> _promotionRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionBranch> _promotionBranchRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionProduct> _promotionProductRepository;

        public PromotionService(IGenericRepository<DAO.Models.Central.Promotion> promotionRepository,
            IGenericRepository<DAO.Models.Central.PromotionBranch> promotionBranchRepository,
            IGenericRepository<DAO.Models.Central.PromotionProduct> promotionProductRepository)
        {
            _promotionRepository = promotionRepository;
            _promotionBranchRepository = promotionBranchRepository;
            _promotionProductRepository = promotionProductRepository;
        }
        public Task<PromotionDTO?> AddPromotion(int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            throw new UnauthorizedAccessException("El usuario final no puede agregar promociones");
        }
        public Task<PromotionDTO?> UpdatePromotion(int promotionId, int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            throw new UnauthorizedAccessException("El usuario final no puede actualizar promociones");
        }

        public PromotionExtendedDTO[] GetPromotionList(int tenantId)
        {
            // Obtener la lista de promociones del repositorio filtrando por TenantId
            var promotions = _promotionRepository.GetQueryable().Where(p => p.TenantId == tenantId);

            //Recorremos las promociones y las agregamos a una lista
            var promotionList = promotions.Select(p => new PromotionExtendedDTO
            {
                PromotionId = p.Id,
                TenantId = p.TenantId,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Branches = _promotionBranchRepository.GetQueryable().Where(pb => pb.TenantId == tenantId && pb.PromotionId == p.Id).Select(pb => pb.BranchId).ToList(),
                Products = _promotionProductRepository.GetQueryable().Where(pp => pp.PromotionId == p.Id).Select(pp => pp.ProductId).ToList(),
                Price = p.Price
            }).ToArray();
            return promotionList;
        }

        public PromotionExtendedDTO GetPromotion(int promotionId, int branchId)
        {
          //Obtenemos promotionBranch
            var  promotionBranch = _promotionBranchRepository.GetQueryable().Where(pb => pb.BranchId == branchId && pb.PromotionId == promotionId).FirstOrDefault();
     
            if (promotionBranch == null)
                throw new Exception($"No existe una promoción con el ID {promotionId} para la sucursal {branchId}");
        
            // Obtener PromotionProduct
            var  promotionProduct = _promotionProductRepository.GetQueryable().Where(pp => pp.PromotionId == promotionId)
                .Select(pp => pp.ProductId)
                .ToList(); 

            var promotion = _promotionRepository.GetQueryable().Where(p => p.TenantId == promotionBranch.TenantId && p.Id == promotionId)
                .FirstOrDefault();
            
            if (promotion == null)
                throw new Exception($"No existe una promoción con el ID {promotionId}");
            
            var promotionExtended = new PromotionExtendedDTO
            {
                PromotionId = promotion.Id,
                TenantId = promotion.TenantId,
                Description = promotion.Description,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Branches = new List<int> { promotionBranch.BranchId },
                Products = promotionProduct,
                Price = promotion.Price
            };

            return promotionExtended;

        }
        public Task<PromotionDTO?> AddPromotionForBranch(int tenantId, int branchId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> product, int price)
        {
            throw new UnauthorizedAccessException("El usuario final no puede agregar promociones para una sucursal");
        }
        
        public PromotionExtendedDTO[] GetBranchPromotionList(int tenantId, int branchId)
        {
            throw new UnauthorizedAccessException("El usuario final no puede obtener promociones de una sucursal específica");
        }
    }

    /// <summary>
    /// Implementación del servicio de productos para el usuario final
    /// </summary>

    /// <summary>
    /// Implementación del servicio de usuarios para el usuario final
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
    /// Implementación del servicio de notificaciones para el usuario final
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
    /// Implementación del servicio de verificacion para el usuario final
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
    /// Implementación del servicio de pagos para el usuario final
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
    /// Implementación del servicio de VEAI para el usuario final
    /// </summary>
    public class VEAIService : IVEAIService
    {
        private readonly WsServicioDeInformacionClient _client;
        private readonly IGenericRepository<DAO.Models.Central.User> _userRepository;

        public VEAIService(IGenericRepository<DAO.Models.Central.User> userRepository)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var endpoint = new EndpointAddress("https://dnic.mz.uy/WsServicioDeInformacion.asmx");
            _client = new WsServicioDeInformacionClient(binding, endpoint);
            _userRepository = userRepository;
        }

        public async Task<UserDTO> VerificarIdentidad(int userId, string nroDoc)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception($"No se encontró el usuario con el ID: {userId}");
            }

            var param = new ParamObtDocDigitalizado
            {
                NroDocumento = nroDoc,
                TipoDocumento = "DO",
                NroSerie = "ABC123456",
                Organismo = "ServiPuntos",
                ClaveAcceso1 = "Clave123"
            };

            var result = await _client.ObtDocDigitalizadoAsync(param);

            var persona = result.Body.ObtDocDigitalizadoResult?.Persona;


            if (persona != null)
            {
                if (DateTime.TryParseExact(persona.FechaNacimiento, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var fechaNacimiento))
                {
                    var hoy = DateTime.Today;
                    var edad = hoy.Year - fechaNacimiento.Year;
                    if (fechaNacimiento > hoy.AddYears(-edad))
                    {
                        edad--;
                    }

                    if (edad >= 18)
                    {
                        user.IsVerified = true;
                        await _userRepository.UpdateAsync(user);
                    }
                }
                else
                {
                    throw new Exception("Formato de fecha de nacimiento inválido.");
                }

                return new UserDTO
                {
                    Id = user.Id,
                    TenantId = user.TenantId.ToString(),
                    Email = user.Email,
                    Name = user.Name,
                    UserType = user.Role,
                    BranchId = user.BranchId,
                    IsVerified = user.IsVerified,
                    PointBalance = user.PointBalance,
                    NotificationsEnabled = user.NotificationsEnabled,
                };
            }
            else
            {
                throw new Exception("Error al verificar la identidad del usuario");
            }
        }
    }

    /// <summary>
    /// Implementación del servicio de productos para el usuario final
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
        public ProductDTO CreateProduct(int tenantId, string name, string description, string imageUrl, decimal price, bool ageRestricted)
        {
            throw new Exception("El usuario final no puede crear productos");
        }


        public async Task<ProductDTO?> GetProductById(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product is not null ? GetProductDTO(product) : null;
        }
        public ProductDTO[] GetProductList(int tenantId)
        {
            var products = _productRepository.GetQueryable().Where(product => product.TenantId == tenantId).ToList();
            return [.. products.Select(GetProductDTO)];
        }
        public async Task<bool> DeleteProduct(int productId)
        {
            throw new Exception("El usuario final no puede eliminar productos");
        }
        public async Task<ProductDTO?> UpdateProduct(int productId, string? name, string? description, string? imageUrl, decimal? price, bool? ageRestricted)
        {
            throw new Exception("El usuario final no puede actualizar productos");
        }
    }

    public class TenantBranchService : ITenantBranchService
    {
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;

        public TenantBranchService(IGenericRepository<DAO.Models.Central.Branch> branchRepository)
        {
            _branchRepository = branchRepository;
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
        public BranchDTO[] GetBranchList(int tenantId)
        {
            // Obtener la lista de branches del repositorio filtrando por TenantId
            var branches = _branchRepository.GetQueryable()
                .Where(e => e.TenantId == tenantId).ToList();

            return branches.Select(b => GetBranchDTO(b)).ToArray();

        }
        public BranchDTO CreateBranch(int tenantId, string latitud, string longitud, string address, string phone, TimeOnly openTime, TimeOnly closingTime)
        {
            throw new UnauthorizedAccessException("El usuario final no puede crear sucursales");
        }

        public BranchDTO UpdateBranch(int branchId, string? latitud, string? longitud, string? address, string? phone, TimeOnly? openTime, TimeOnly? closingTime)
        {
            throw new UnauthorizedAccessException("El usuario final no puede actualizar sucursales");
        }

        public void DeleteBranch(int branchId)
        {
            throw new UnauthorizedAccessException("El usuario final no puede eliminar sucursales");
        }
    }

    public class TransactionService : ITransactionService {

        private readonly DbContext _dbContext;
        private readonly IGenericRepository<DAO.Models.Central.Transaction> _transactionRepository;
        private readonly IGenericRepository<DAO.Models.Central.LoyaltyConfig> _loyaltyConfigRepository;
        private readonly IGenericRepository<DAO.Models.Central.Product> _productRepository;
        private readonly IGenericRepository<DAO.Models.Central.TransactionItem> _transactionItemRepository;
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly IGenericRepository<DAO.Models.Central.ProductStock> _productStockRepository;
        private readonly IGenericRepository<DAO.Models.Central.User> _userRepository;

        public TransactionService(
            DbContext dbContext,
            IGenericRepository<DAO.Models.Central.Transaction> transactionRepository,
            IGenericRepository<DAO.Models.Central.LoyaltyConfig> loyaltyConfigRepository,
            IGenericRepository<DAO.Models.Central.Product> productRepository,
            IGenericRepository<DAO.Models.Central.TransactionItem> transactionItemRepository,
            IGenericRepository<DAO.Models.Central.Branch> branchRepository,
            IGenericRepository<DAO.Models.Central.ProductStock> productStockRepository,
            IGenericRepository<DAO.Models.Central.User> userRepository
        )
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _loyaltyConfigRepository = loyaltyConfigRepository ?? throw new ArgumentNullException(nameof(loyaltyConfigRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _transactionItemRepository = transactionItemRepository ?? throw new ArgumentNullException(nameof(transactionItemRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
            _productStockRepository = productStockRepository ?? throw new ArgumentNullException(nameof(productStockRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        /// <summary>
        /// Obtiene los items (productos) de una transacción específica
        /// </summary>
        /// <param name="transactionId">ID de la transacción</param>
        /// <returns>Array de items de la transacción</returns>
        public async Task<TransactionItemDTO[]> GetTransactionItems(int transactionId)
        {
            // Verificar que la transacción existe
            var transaction = await _transactionRepository.GetByIdAsync(transactionId);
            if (transaction == null)
            {
                throw new Exception($"No existe una transacción con el ID {transactionId}");
            }

            // Obtener los items de la transacción
            var items = await _transactionItemRepository.GetQueryable()
                .Where(ti => ti.TransactionId == transactionId)
                .ToListAsync();

            // Obtener los productos asociados a los items
            var productIds = items.Select(i => i.ProductId).ToArray();
            var products = await _productRepository.GetQueryable()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            // Mapear a DTOs
            return items.Select(item => new TransactionItemDTO
            {
                Id = item.Id,
                TransactionId = item.TransactionId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                ProductName = products.TryGetValue(item.ProductId, out var product) ? product.Name : "Producto no encontrado",
                ProductImageUrl = products.TryGetValue(item.ProductId, out var productForImage) ? productForImage.ImageUrl : null
            }).ToArray();
        }

        public TransactionDTO GetTransactionDTO(Transaction transaction)
        {
            return new TransactionDTO {
                Id = transaction.Id,
                UserId = transaction.UserId,
                BranchId = transaction.BranchId,
                Amount = transaction.Amount,
                PointsEarned = transaction.PointsEarned,
                PointsSpent = transaction.PointsSpent,
                Type = transaction.Type,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<TransactionDTO> CreateTransaction(int userId, int branchId, ProductQuantity[] products)
        {
            try {
                // Obtener los productos
                var productIds = products.Select(p => p.ProductId).ToArray();

                var productsList = await _productRepository.GetQueryable()
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                // Obtener la branch
                var branch = await _branchRepository.GetQueryable()
                    .FirstOrDefaultAsync(b => b.Id == branchId);

                if (branch == null)
                {
                    throw new Exception($"No se encontró la sucursal con ID {branchId}");
                }

                // Verificar el stock de cada producto
                foreach (var product in products)
                {
                    var productStock = await _productStockRepository.GetQueryable()
                        .FirstOrDefaultAsync(ps => ps.ProductId == product.ProductId && ps.BranchId == branchId);

                    if (productStock == null)
                    {
                        throw new Exception($"No hay stock registrado para el producto {product.ProductId} en la sucursal {branchId}");
                    }

                    if (productStock.Stock < product.Quantity)
                    {
                        throw new Exception($"Stock insuficiente para el producto {product.ProductId}. Stock disponible: {productStock.Stock}, Cantidad solicitada: {product.Quantity}");
                    }
                }

                // Buscar la configuración de lealtad del tenant
                var loyaltyConfig = await _loyaltyConfigRepository.GetQueryable()
                    .FirstOrDefaultAsync(lc => lc.TenantId == branch.TenantId);

                if (loyaltyConfig == null) {
                    throw new Exception("La configuración de lealtad no existe para este tenant");
                }

                // Calcular el monto total considerando las cantidades
                decimal totalAmount = 0;
                foreach (var product in products)
                {
                    var productInfo = productsList.First(p => p.Id == product.ProductId);
                    totalAmount += productInfo.Price * product.Quantity;
                }

                // Crear la transacción
                var transaction = new Transaction {
                    UserId = userId,
                    BranchId = branchId,
                    Amount = totalAmount,
                    PointsEarned = (int)(totalAmount / loyaltyConfig.AccumulationRule),
                    PointsSpent = 0,
                    Type = TransactionType.Purchase,
                    CreatedAt = DateTime.UtcNow
                };

                // Actualizar usuario y agregar puntos
                var user = await _userRepository.GetQueryable()
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception($"No se encontró el usuario con ID {userId}");
                }
                user.PointBalance += transaction.PointsEarned;

                // Iniciar una transacción de base de datos
                using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    // Guardar la transacción
                    await _transactionRepository.AddAsync(transaction);
                    await _transactionRepository.SaveChangesAsync();

                    // Crear las relaciones entre la transacción y los productos
                    foreach (var item in productsList)
                    {
                        var transactionProduct = new TransactionItem
                        {
                            TransactionId = transaction.Id,
                            ProductId = item.Id,
                            Quantity = products.First(p => p.ProductId == item.Id).Quantity,
                            UnitPrice = item.Price
                        };

                        await _transactionItemRepository.AddAsync(transactionProduct);
                    }

                    // Actualizar el stock de los productos
                    foreach (var product in products)
                    {
                        var productStock = await _productStockRepository.GetQueryable()
                            .FirstOrDefaultAsync(ps => ps.ProductId == product.ProductId && ps.BranchId == branchId);

                        productStock.Stock -= product.Quantity;
                        await _productStockRepository.UpdateAsync(productStock);
                    }

                    // Guardar todos los cambios de una vez
                    await _transactionItemRepository.SaveChangesAsync();
                    await _productStockRepository.SaveChangesAsync();

                    // Confirmar la transacción
                    await dbTransaction.CommitAsync();

                    // Devolver la transacción
                    return GetTransactionDTO(transaction);
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir la transacción
                    await dbTransaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TransactionDTO> GetTransactionById(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);

            if (transaction == null) {
                throw new Exception("La transacción no existe");
            }

            return GetTransactionDTO(transaction);
        }

        public async Task<TransactionDTO[]> GetTransactionsByUserId(int userId)
        {
            var transactions = await _transactionRepository.GetQueryable()
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return transactions.Select(GetTransactionDTO).ToArray();
        }

    }

    /// <summary>
    /// Implementación del servicio de canjes para el usuario final
    /// </summary>
    public class RedemptionService : IRedemptionService
    {
        private readonly DbContext _dbContext;
        private readonly IGenericRepository<Transaction> _transactionRepository;
        private readonly IGenericRepository<TransactionItem> _transactionItemRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductStock> _productStockRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<LoyaltyConfig> _loyaltyConfigRepository;
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly IConfiguration _configuration;

        public RedemptionService(
            DbContext dbContext,
            IGenericRepository<Transaction> transactionRepository,
            IGenericRepository<TransactionItem> transactionItemRepository,
            IGenericRepository<Product> productRepository,
            IGenericRepository<ProductStock> productStockRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<LoyaltyConfig> loyaltyConfigRepository,
            IGenericRepository<DAO.Models.Central.Branch> branchRepository,
            IConfiguration configuration)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            _transactionItemRepository = transactionItemRepository ?? throw new ArgumentNullException(nameof(transactionItemRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productStockRepository = productStockRepository ?? throw new ArgumentNullException(nameof(productStockRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _loyaltyConfigRepository = loyaltyConfigRepository ?? throw new ArgumentNullException(nameof(loyaltyConfigRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Genera un token JWT con la información del canje
        /// </summary>
        public async Task<string> GenerateRedemptionToken(int userId, int branchId, int productId)
        {
            // 1. Verificar que el usuario exista
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            // 2. Verificar que el producto exista y tenga stock
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Producto no encontrado");
            }

            var productStock = await _productStockRepository.GetQueryable()
                .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.BranchId == branchId);

            if (productStock == null || productStock.Stock < 1)
            {
                throw new Exception("Stock insuficiente para el producto");
            }

            // 3. Obtener la configuración de lealtad
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null)
            {
                throw new Exception("Sucursal no encontrada");
            }

            var loyaltyConfig = await _loyaltyConfigRepository.GetQueryable()
                .FirstOrDefaultAsync(lc => lc.TenantId == branch.TenantId);

            if (loyaltyConfig == null)
            {
                throw new Exception("Configuración de lealtad no encontrada");
            }

            // 4. Calcular el costo en puntos
            int pointsCost = (int)(product.Price / loyaltyConfig.PointsValue);

            if (user.PointBalance < pointsCost)
            {
                throw new Exception("Puntos insuficientes para realizar el canje");
            }

            // 5. Crear el payload del token
            var expiresAt = DateTime.UtcNow.AddMinutes(15); // El token expira en 15 minutos

            // 6. Generar el token JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("userId", userId.ToString()),
                    new Claim("branchId", branchId.ToString()),
                    new Claim("productId", productId.ToString()),
                    new Claim("pointsCost", pointsCost.ToString()),
                    new Claim("expiresAt", expiresAt.ToString("o"))
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Procesa un canje a partir de un token
        /// </summary>
        public async Task<TransactionDTO> ProcessRedemption(string token)
        {
            try
            {
                // 1. Validar y decodificar el token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                
                // 2. Extraer la información del token
                var userId = int.Parse(principal.FindFirst("userId").Value);
                var branchId = int.Parse(principal.FindFirst("branchId").Value);
                var productId = int.Parse(principal.FindFirst("productId").Value);
                var pointsCost = int.Parse(principal.FindFirst("pointsCost").Value);
                var expiresAt = DateTime.Parse(principal.FindFirst("expiresAt").Value);

                // 3. Verificar que el token no haya expirado
                if (expiresAt > DateTime.UtcNow)
                {
                    throw new Exception("El token ha expirado");
                }

                // 4. Verificar que el usuario exista y tenga suficientes puntos
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                if (user.PointBalance < pointsCost)
                {
                    throw new Exception("Puntos insuficientes para realizar el canje");
                }

                // 5. Verificar que el producto exista y tenga stock
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new Exception("Producto no encontrado");
                }

                var productStock = await _productStockRepository.GetQueryable()
                    .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.BranchId == branchId);

                if (productStock == null || productStock.Stock < 1)
                {
                    throw new Exception("Stock insuficiente para el producto");
                }

                // 6. Iniciar transacción de base de datos
                using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();

                try
                {
                    // 7. Crear la transacción
                    var transaction = new Transaction
                    {
                        UserId = userId,
                        BranchId = branchId,
                        Amount = 0, // No hay monto en dinero
                        PointsEarned = 0, // No se ganan puntos
                        PointsSpent = pointsCost, // Se gastan puntos
                        Type = TransactionType.Redemption,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _transactionRepository.AddAsync(transaction);
                    await _transactionRepository.SaveChangesAsync();

                    // 8. Crear el item de transacción
                    var transactionItem = new TransactionItem
                    {
                        TransactionId = transaction.Id,
                        ProductId = productId,
                        Quantity = 1,
                        UnitPrice = product.Price
                    };

                    await _transactionItemRepository.AddAsync(transactionItem);

                    // 9. Actualizar el stock
                    productStock.Stock -= 1;
                    await _productStockRepository.UpdateAsync(productStock);

                    // 10. Restar puntos al usuario
                    user.PointBalance -= pointsCost;
                    await _userRepository.UpdateAsync(user);

                    // 11. Guardar todos los cambios
                    await _transactionItemRepository.SaveChangesAsync();
                    await _productStockRepository.SaveChangesAsync();
                    await _userRepository.SaveChangesAsync();

                    // 12. Confirmar la transacción
                    await dbTransaction.CommitAsync();

                    // 13. Devolver la transacción
                    return new TransactionDTO
                    {
                        Id = transaction.Id,
                        UserId = transaction.UserId,
                        BranchId = transaction.BranchId,
                        Amount = transaction.Amount,
                        PointsEarned = transaction.PointsEarned,
                        PointsSpent = transaction.PointsSpent,
                        Type = transaction.Type,
                        CreatedAt = transaction.CreatedAt
                    };
                }
                catch (Exception ex)
                {
                    // Si algo falla, revertir la transacción
                    await dbTransaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar el canje: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Implementación del servicio de parámetros generales para el usuario final
    /// </summary>
    public class GeneralParameterService : IGeneralParameterService
    {
        private readonly IGenericRepository<GeneralParameter> _generalParameterRepository;

        public GeneralParameterService(IGenericRepository<GeneralParameter> generalParameterRepository)
        {
            _generalParameterRepository = generalParameterRepository;
        }

        /// <summary>
        /// Convierte un modelo de parámetro general a DTO
        /// </summary>
        /// <param name="parameter">Modelo de parámetro general</param>
        /// <returns>DTO de parámetro general</returns>
        private GeneralParameterDTO GetGeneralParameterDTO(GeneralParameter parameter)
        {
            return new GeneralParameterDTO
            {
                Id = parameter.Id,
                Key = parameter.Key,
                Value = parameter.Value,
                Description = parameter.Description
            };
        }

        /// <summary>
        /// Obtiene un parámetro general por su clave
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <returns>DTO del parámetro general</returns>
        public GeneralParameterDTO GetParameter(string key)
        {
            var parameter = _generalParameterRepository.GetQueryable()
                .FirstOrDefault(p => p.Key == key);

            if (parameter == null)
            {
                throw new ArgumentException($"No existe un parámetro con la clave '{key}'");
            }

            return GetGeneralParameterDTO(parameter);
        }

        /// <summary>
        /// Obtiene todos los parámetros generales
        /// </summary>
        /// <returns>Array de DTOs de parámetros generales</returns>
        public GeneralParameterDTO[] GetAllParameters()
        {
            var parameters = _generalParameterRepository.GetQueryable().ToList();
            return parameters.Select(GetGeneralParameterDTO).ToArray();
        }

        /// <summary>
        /// Actualiza un parámetro general existente (no permitido para usuarios finales)
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <param name="value">Nuevo valor</param>
        /// <param name="description">Nueva descripción (opcional)</param>
        /// <returns>DTO del parámetro actualizado</returns>
        public GeneralParameterDTO UpdateParameter(string key, string value, string description = null)
        {
            throw new UnauthorizedAccessException("El usuario final no puede modificar parámetros generales");
        }

        /// <summary>
        /// Crea un nuevo parámetro general (no permitido para usuarios finales)
        /// </summary>
        /// <param name="key">Clave del parámetro</param>
        /// <param name="value">Valor del parámetro</param>
        /// <param name="description">Descripción del parámetro</param>
        /// <returns>DTO del parámetro creado</returns>
        public GeneralParameterDTO CreateParameter(string key, string value, string description)
        {
            throw new UnauthorizedAccessException("El usuario final no puede crear parámetros generales");
        }
    }

    /// <summary>
    /// Implementación del servicio de gestión de servicios para el usuario final
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IGenericRepository<ServiceAvailability> _serviceAvailabilityRepository;
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;

        public ServiceManager(
            IGenericRepository<Service> serviceRepository,
            IGenericRepository<ServiceAvailability> serviceAvailabilityRepository,
            IGenericRepository<DAO.Models.Central.Branch> branchRepository)
        {
            _serviceRepository = serviceRepository;
            _serviceAvailabilityRepository = serviceAvailabilityRepository;
            _branchRepository = branchRepository;
        }

        private ServiceDTO MapToServiceDTO(Service service)
        {
            return new ServiceDTO
            {
                Id = service.Id,
                TenantId = service.TenantId,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                AgeRestricted = service.AgeRestricted
            };
        }

        private ServiceAvailabilityDTO MapToServiceAvailabilityDTO(ServiceAvailability availability)
        {
            return new ServiceAvailabilityDTO
            {
                Id = availability.Id,
                BranchId = availability.BranchId,
                ServiceId = availability.ServiceId,
                TenantId = availability.TenantId,
                StartTime = availability.StartTime,
                EndTime = availability.EndTime
            };
        }

        // Implementación de los métodos de solo lectura
        public async Task<ServiceDTO> GetServiceByIdAsync(int serviceId)
        {
            // Obtener el servicio
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
                throw new Exception($"No existe un servicio con el ID {serviceId}");

            // Obtener las disponibilidades para este servicio
            var availabilities = await _serviceAvailabilityRepository.GetQueryable()
                .Where(sa => sa.ServiceId == serviceId)
                .ToListAsync();

            // Mapear el servicio a DTO
            var serviceDTO = MapToServiceDTO(service);

            // Agregar las disponibilidades al DTO
            serviceDTO.Availabilities = availabilities.Select(a => {
                var dto = MapToServiceAvailabilityDTO(a);
                dto.ServiceName = service.Name;
                return dto;
            }).ToArray();

            return serviceDTO;
        }

        public async Task<ServiceDTO[]> GetBranchServicesAsync(int branchId)
        {
            // Obtener las disponibilidades para esta branch
            var availabilities = await _serviceAvailabilityRepository.GetQueryable()
                .Where(sa => sa.BranchId == branchId)
                .ToListAsync();

            // Agrupar las disponibilidades por servicio
            var serviceAvailabilities = availabilities
                .GroupBy(a => a.ServiceId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Obtener los IDs de los servicios disponibles en esta branch
            var serviceIds = serviceAvailabilities.Keys.ToArray();

            // Obtener los servicios correspondientes
            var services = await _serviceRepository.GetQueryable()
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync();

            // Mapear los servicios a DTOs incluyendo sus disponibilidades
            return services.Select(s => {
                var serviceDTO = MapToServiceDTO(s);

                // Agregar las disponibilidades al DTO
                if (serviceAvailabilities.ContainsKey(s.Id))
                {
                    serviceDTO.Availabilities = serviceAvailabilities[s.Id].Select(a => {
                        var dto = MapToServiceAvailabilityDTO(a);
                        dto.ServiceName = s.Name;
                        return dto;
                    }).ToArray();
                }

                return serviceDTO;
            }).ToArray();
        }

        // Métodos no implementados para el usuario final (lanzarán excepciones)
        public Task<ServiceDTO> CreateServiceAsync(int branchId, string name, string description, decimal price, bool ageRestricted, TimeOnly startTime, TimeOnly endTime)
        {
            throw new UnauthorizedAccessException("Los usuarios finales no pueden crear servicios");
        }

        public Task<ServiceDTO> UpdateServiceAsync(int serviceId, string name, string description, decimal price, bool ageRestricted, TimeOnly? startTime = null, TimeOnly? endTime = null)
        {
            throw new UnauthorizedAccessException("Los usuarios finales no pueden actualizar servicios");
        }

        public Task<bool> DeleteServiceAsync(int serviceId)
        {
            throw new UnauthorizedAccessException("Los usuarios finales no pueden eliminar servicios");
        }

    }
}
