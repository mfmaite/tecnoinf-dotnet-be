using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.CommonLogic;

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

        public List<FuelPrices> GetAllFuelPrices(int branchId)
        {
            // Verificar que el usuario solo pueda obtener precios de su propia estación
            if (branchId != _branchId)
            {
                throw new UnauthorizedAccessException("Solo puede obtener precios de su propia estación");
            }

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

        public async Task<ProductStockDTO?> manageStock(int productId, int branchId, int stock)
        {
            try
            {
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
                }
                else
                {
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

        public async Task<BranchDTO?> setBranchHours(int id, TimeOnly openTime, TimeOnly closingTime)
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

        public async Task<ProductWithStockDTO[]> GetBranchProductsWithStock(int branchId, int tenantId)
        {
            try
            {
                // Obtener todos los productos del tenant
                var products = await _productRepository.GetQueryable()
                    .Where(p => p.TenantId == tenantId)
                    .AsNoTracking()
                    .ToListAsync();

                // Obtener el stock para cada producto en este branch
                var productStocks = await _productStockRepository.GetQueryable()
                    .Where(ps => ps.BranchId == branchId)
                    .AsNoTracking()
                    .ToDictionaryAsync(ps => ps.ProductId, ps => ps.Stock);

                // Mapear a DTOs
                return products.Select(p => new ProductWithStockDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,
                    AgeRestricted = p.AgeRestricted,
                    Stock = productStocks.ContainsKey(p.Id) ? productStocks[p.Id] : 0
                }).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos con stock: {ex.Message}");
                throw;
            }
        }
    }

    /// <summary>
    /// Implementación del servicio de lealtad para el administrador de branch
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

        public LoyaltyConfigDTO CreateLoyaltyProgram(int tenantId, string pointsName, int pointsValue, decimal accumulationRule, int expiricyPolicyDays)
        {
            throw new UnauthorizedAccessException("El administrador de branch no puede crear un programa de fidelidad");
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

        /// <summary>
        /// Obtiene la configuración de lealtad de un tenant
        /// </summary>
        /// <param name="tenantId">ID del tenant</param>
        /// <returns>Configuración de lealtad</returns>
        public LoyaltyConfigDTO GetLoyaltyProgram(int tenantId)
        {
            var loyaltyConfig = _loyaltyConfigRepository.GetQueryable().FirstOrDefault(lc => lc.TenantId == tenantId);

            if (loyaltyConfig == null)
            {
                throw new ArgumentException($"No existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            return new LoyaltyConfigDTO
            {
                Id = loyaltyConfig.Id,
                TenantId = loyaltyConfig.TenantId,
                PointsName = loyaltyConfig.PointsName,
                PointsValue = loyaltyConfig.PointsValue,
                AccumulationRule = loyaltyConfig.AccumulationRule,
                ExpiricyPolicyDays = loyaltyConfig.ExpiricyPolicyDays
            };
        }


        public LoyaltyConfigDTO UpdateLoyaltyProgram(int tenantId, string? pointsName, int? pointsValue, decimal? accumulationRule, int? expiricyPolicyDays)
        {
            throw new UnauthorizedAccessException("El admin branch no puede actualizar un programa de fidelidad");
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
        public Task<PromotionDTO?> AddPromotion(int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }
        public Task<PromotionDTO?> UpdatePromotion(int promotionId, int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        public PromotionExtendedDTO[] GetPromotionList(int tenantId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }

        public PromotionExtendedDTO GetPromotion(int promotionId, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }
        public Task<PromotionDTO?> AddPromotionForBranch(int tenantId, int branchId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> product, int price)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }
        public PromotionExtendedDTO[] GetBranchPromotionList(int tenantId, int branchId)
        {
            // Implementación básica para el scaffold
            throw new NotImplementedException();
        }
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

    /// <summary>
    /// Implementación del servicio de estadísticas para el administrador de branch
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;
        private readonly int _branchId;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StatisticsService(
            DbContext dbContext,
            IConfiguration configuration,
            ITenantAccessor tenantAccessor,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantAccessor.GetCurrentTenantId();
            _httpContextAccessor = httpContextAccessor;

            // Obtener el branchId del contexto HTTP
            if (httpContextAccessor.HttpContext?.Items["BranchId"] is int branchId)
            {
                _branchId = branchId;
            }
            else
            {
                throw new ArgumentException("No se pudo obtener el BranchId del contexto");
            }
        }

        /// <summary>
        /// Obtiene las estadísticas para el administrador de branch
        /// </summary>
        /// <returns>Estadísticas específicas del branch</returns>
        public async Task<object> GetStatisticsAsync()
        {
            // Contar promociones para este branch
            int branchPromotions = await _dbContext.Set<DAO.Models.Central.Promotion>()
                .Where(p => p.TenantId == int.Parse(_tenantId) &&
                            p.PromotionBranch.Any(pb => pb.BranchId == _branchId))
                .CountAsync();

            // Construir el objeto de respuesta
            var statistics = new
            {
                promotions = new
                {
                    total = branchPromotions
                }
            };

            return statistics;
        }
    }

    /// <summary>
    /// Implementación del servicio de gestión de servicios para el administrador de branch
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        private readonly IGenericRepository<Service> _serviceRepository;
        private readonly IGenericRepository<ServiceAvailability> _serviceAvailabilityRepository;
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly int _branchId;
        private readonly int _tenantId;

        public ServiceManager(
            IGenericRepository<Service> serviceRepository,
            IGenericRepository<ServiceAvailability> serviceAvailabilityRepository,
            IGenericRepository<DAO.Models.Central.Branch> branchRepository,
            int branchId,
            int tenantId)
        {
            _serviceRepository = serviceRepository;
            _serviceAvailabilityRepository = serviceAvailabilityRepository;
            _branchRepository = branchRepository;
            _branchId = branchId;
            _tenantId = tenantId;
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

        private async Task ValidateServiceHours(int branchId, TimeOnly startTime, TimeOnly endTime)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);

            if (branch == null)
                throw new Exception($"No existe una branch con el ID {branchId}");

            if (startTime < branch.OpenTime)
                throw new Exception($"La hora de inicio del servicio ({startTime}) debe ser posterior a la hora de apertura de la branch ({branch.OpenTime})");

            if (endTime > branch.ClosingTime)
                throw new Exception($"La hora de fin del servicio ({endTime}) debe ser anterior a la hora de cierre de la branch ({branch.ClosingTime})");

            if (startTime >= endTime)
                throw new Exception("La hora de inicio debe ser anterior a la hora de fin");
        }

        public async Task<ServiceDTO> CreateServiceAsync(int branchId, string name, string description, decimal price, bool ageRestricted, TimeOnly startTime, TimeOnly endTime)
        {

            // Validar que las horas estén dentro del horario de la branch
            await ValidateServiceHours(branchId, startTime, endTime);

            // Crear el servicio
            var service = new Service
            {
                TenantId = _tenantId,
                Name = name,
                Description = description,
                Price = price,
                AgeRestricted = ageRestricted
            };

            var createdService = await _serviceRepository.AddAsync(service);
            await _serviceRepository.SaveChangesAsync();

            // Crear la disponibilidad para el servicio
            var serviceAvailability = new ServiceAvailability
            {
                BranchId = branchId,
                ServiceId = createdService.Id,
                TenantId = _tenantId,
                StartTime = startTime,
                EndTime = endTime
            };

            await _serviceAvailabilityRepository.AddAsync(serviceAvailability);
            await _serviceAvailabilityRepository.SaveChangesAsync();

            // Devolver el servicio creado con su disponibilidad
            return await GetServiceByIdAsync(createdService.Id);
        }

        public async Task<ServiceDTO> UpdateServiceAsync(int serviceId, string name, string description, decimal price, bool ageRestricted, TimeOnly? startTime = null, TimeOnly? endTime = null)
        {
            // Obtener el servicio existente
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
                throw new Exception($"No existe un servicio con el ID {serviceId}");

            // Verificar que el servicio pertenezca al tenant del usuario
            if (service.TenantId != _tenantId)
                throw new UnauthorizedAccessException("No tiene permiso para modificar este servicio");

            // Verificar que el servicio esté asociado a la branch del usuario
            var serviceAvailability = await _serviceAvailabilityRepository.GetQueryable()
                .FirstOrDefaultAsync(sa => sa.ServiceId == serviceId && sa.BranchId == _branchId);
            if (serviceAvailability == null)
                throw new UnauthorizedAccessException("Este servicio no está asociado a su branch");

            // Actualizar el servicio
            service.Name = name;
            service.Description = description;
            service.Price = price;
            service.AgeRestricted = ageRestricted;

            await _serviceRepository.UpdateAsync(service);
            await _serviceRepository.SaveChangesAsync();

            // Si se proporcionaron horarios, actualizar la disponibilidad
            if (startTime.HasValue && endTime.HasValue)
            {
                // Validar que las horas estén dentro del horario de la branch
                await ValidateServiceHours(_branchId, startTime.Value, endTime.Value);

                // Actualizar la disponibilidad
                serviceAvailability.StartTime = startTime.Value;
                serviceAvailability.EndTime = endTime.Value;

                await _serviceAvailabilityRepository.UpdateAsync(serviceAvailability);
                await _serviceAvailabilityRepository.SaveChangesAsync();
            }

            // Devolver el servicio actualizado con su disponibilidad
            return await GetServiceByIdAsync(serviceId);
        }

        public async Task<bool> DeleteServiceAsync(int serviceId)
        {
            // Obtener el servicio existente
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
                return false;

            // Verificar que el servicio pertenezca al tenant del usuario
            if (service.TenantId != _tenantId)
                throw new UnauthorizedAccessException("No tiene permiso para eliminar este servicio");

            // Verificar que el servicio esté asociado a la branch del usuario
            var serviceAvailability = await _serviceAvailabilityRepository.GetQueryable()
                .FirstOrDefaultAsync(sa => sa.ServiceId == serviceId && sa.BranchId == _branchId);
            if (serviceAvailability == null)
                throw new UnauthorizedAccessException("Este servicio no está asociado a su branch");

            // Eliminar primero las disponibilidades asociadas
            var availabilities = await _serviceAvailabilityRepository.GetQueryable()
                .Where(sa => sa.ServiceId == serviceId)
                .ToListAsync();

            foreach (var availability in availabilities)
            {
                await _serviceAvailabilityRepository.DeleteAsync(availability.Id);
            }

            // Eliminar el servicio
            await _serviceRepository.DeleteAsync(serviceId);
            await _serviceRepository.SaveChangesAsync();

            return true;
        }


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
            serviceDTO.Availabilities = availabilities.Select(a =>
            {
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
            return services.Select(s =>
            {
                var serviceDTO = MapToServiceDTO(s);

                // Agregar las disponibilidades al DTO
                if (serviceAvailabilities.ContainsKey(s.Id))
                {
                    serviceDTO.Availabilities = serviceAvailabilities[s.Id].Select(a =>
                    {
                        var dto = MapToServiceAvailabilityDTO(a);
                        dto.ServiceName = s.Name;
                        return dto;
                    }).ToArray();
                }

                return serviceDTO;
            }).ToArray();
        }
    }
}
