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
using FirebaseAdmin.Messaging;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ServiPuntosUy.DataServices.Services.Tenant
{
    public class TenantBranchService : ITenantBranchService
    {
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly IGenericRepository<DAO.Models.Central.FuelPrices> _fuelPricesRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IAuthLogic _authLogic;


        public TenantBranchService(
            IGenericRepository<DAO.Models.Central.Branch> branchRepository,
            IGenericRepository<DAO.Models.Central.FuelPrices> fuelPricesRepository,
            IGenericRepository<User> userRepository,
            IAuthLogic authLogic)
        {
            _branchRepository = branchRepository;
            _fuelPricesRepository = fuelPricesRepository;
            _userRepository = userRepository;
            _authLogic = authLogic;
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

        private void CreateBranchAdminUser(DAO.Models.Central.Branch branch)
        {
            // Generar email y contraseña para el administrador del tenant
            string adminEmail = $"branch{branch.Id}tenant{branch.TenantId}-admin@mail.com";
            string adminPassword = $"branch{branch.Id}tenant{branch.TenantId}-admin-password";

            // Generar hash y salt para la contraseña
            string passwordHash = _authLogic.HashPassword(adminPassword, out string passwordSalt);

            // Crear el usuario administrador
            var adminUser = new User
            {
                TenantId = branch.TenantId,
                Email = adminEmail,
                Name = $"Default branch{branch.Id}tenant{branch.TenantId} Administrator",
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserType.Branch, // Tipo de usuario: branch admin
                IsVerified = true, // El usuario ya está verificado
                NotificationsEnabled = true,
                BranchId = branch.Id

            };

            // Guardar el usuario en la base de datos
            try
            {
                var createdUser = _userRepository.AddAsync(adminUser).GetAwaiter().GetResult();
                _userRepository.SaveChangesAsync().GetAwaiter().GetResult();

                if (createdUser == null)
                {
                    throw new Exception("No se pudo crear el usuario administrador del branch.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear el usuario administrador del branch: {ex.Message}");
            }
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

            // imprimimos el tipo de createdBranch
            // Crear el usuario administrador del branch
            CreateBranchAdminUser(createdBranch);

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

            var users = _userRepository.GetQueryable().Where(u => u.BranchId == branchId).ToList();
            foreach (var user in users)
            {
                _userRepository.DeleteAsync(user.Id).GetAwaiter().GetResult();
            }
            _userRepository.SaveChangesAsync().GetAwaiter().GetResult();

            _branchRepository.DeleteAsync(branchId).GetAwaiter().GetResult();
            _branchRepository.SaveChangesAsync().GetAwaiter().GetResult();


        }

        public BranchDTO setBranchHours(int id, TimeOnly openTime, TimeOnly closingTiem)
        {
            throw new NotImplementedException("Este método no está implementado en la versión actual del servicio.");
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

            if (existingLoyaltyConfig == null)
            {
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

            if (existingLoyaltyConfig != null)
            {
                throw new Exception($"Ya existe una configuración de lealtad para el tenant con el ID {tenantId}");
            }

            // Crear la configuración de lealtad para el tenant
            var newLoyaltyConfig = new DAO.Models.Central.LoyaltyConfig
            {
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
        public LoyaltyConfigDTO GetLoyaltyConfigDTO(LoyaltyConfig loyaltyConfig)
        {
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

            return GetLoyaltyConfigDTO(loyaltyConfig);
        }

    }

    /// <summary>
    /// Implementación del servicio de promociones para el administrador de tenant
    /// </summary>
    public class PromotionService : IPromotionService
    {
        private readonly IGenericRepository<DAO.Models.Central.Promotion> _promotionRepository;
        private readonly IGenericRepository<DAO.Models.Central.Tenant> _tenantRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionBranch> _promotionBranchRepository;
        private readonly IGenericRepository<DAO.Models.Central.PromotionProduct> _promotionProductRepository;
        private readonly IGenericRepository<DAO.Models.Central.Branch> _branchRepository;
        private readonly IGenericRepository<DAO.Models.Central.Product> _productRepository;
        private readonly IGenericRepository<DAO.Models.Central.GeneralParameter> _generalParameterRepository;
        public PromotionService(IGenericRepository<DAO.Models.Central.Promotion> promotionRepository,
                                IGenericRepository<DAO.Models.Central.PromotionProduct> promotionProductRepository,
                                IGenericRepository<DAO.Models.Central.PromotionBranch> promotionBranchRepository,
                                IGenericRepository<DAO.Models.Central.Branch> branchRepository,
                                IGenericRepository<DAO.Models.Central.Product> productRepository,
                                IGenericRepository<DAO.Models.Central.Tenant> tenantRepository,
                                IGenericRepository<DAO.Models.Central.GeneralParameter> generalParameterRepository
                                )

        {
            _promotionBranchRepository = promotionBranchRepository;
            _promotionProductRepository = promotionProductRepository;
            _promotionRepository = promotionRepository;
            _branchRepository = branchRepository;
            _productRepository = productRepository;
            _tenantRepository = tenantRepository;
            _generalParameterRepository = generalParameterRepository;
        }

        public async Task<PromotionDTO?> GetPrmotionById(int promotionId)
        {
            // Obtener la promoción por ID
            var promotion = await _promotionRepository.GetQueryable()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == promotionId);

            return promotion is not null ? GetPromotionDTO(promotion) : null;

        }

        // map promotionDTO con DAO.Models.Central.Promotion
        private DAO.Models.Central.Promotion MapToPromotion(PromotionDTO promotionDTO)
        {
            return new DAO.Models.Central.Promotion
            {
                Id = promotionDTO.Id,
                TenantId = promotionDTO.TenantId,
                Description = promotionDTO.Description,
                StartDate = promotionDTO.StartDate,
                EndDate = promotionDTO.EndDate,
                Price = promotionDTO.Price
            };
        }


        public PromotionDTO GetPromotionDTO(DAO.Models.Central.Promotion promotion)
        {
            return new PromotionDTO
            {
                Id = promotion.Id,
                TenantId = promotion.TenantId,
                Description = promotion.Description,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                Price = promotion.Price
            };
        }

        // Creamos una funcion para verificar los productos 
        private void verifyProductList(IEnumerable<int> product)
        {
            var productIds = product.ToList();
            // Validaciones previas - verificar existencia de productos
            if (productIds.Any())
            {
                var existingProductIds = _productRepository.GetQueryable()
                    .Where(p => productIds.Contains(p.Id))
                    .Select(p => p.Id)
                    .Distinct()
                    .ToList();

                var missingProductIds = productIds.Except(existingProductIds).ToList();
                if (missingProductIds.Any())
                    throw new Exception($"No existen productos con los IDs: {string.Join(", ", missingProductIds)} para el tenant");
            }
        }

        private void verifyBranchList(IEnumerable<int> branch, int tenantId)
        {
            var branchIds = branch.ToList();
            if (branchIds.Any())
            {
                var existingBranchIds = _branchRepository.GetQueryable()
                    .Where(b => branchIds.Contains(b.Id) && b.TenantId == tenantId)
                    .Select(b => b.Id)
                    .Distinct()
                    .ToList();

                var missingBranchIds = branchIds.Except(existingBranchIds).ToList();
                if (missingBranchIds.Any())
                    throw new Exception($"No existen branches con los IDs: {string.Join(", ", missingBranchIds)} para el tenant");
            }
        }


        public async Task<PromotionDTO?> AddPromotion(int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            // Validaciones previas - verificar existencia de branches
            verifyBranchList(branch, tenantId);
            // Validaciones previas - verificar existencia de productos
            verifyProductList(product);

            var promotion = new DAO.Models.Central.Promotion
            {
                TenantId = tenantId,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                Price = price
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
                    BranchId = bId,
                    TenantId = tenantId
                };
                _promotionBranchRepository.AddAsync(promotionBranch).GetAwaiter().GetResult();
            }
            _promotionBranchRepository.SaveChangesAsync().GetAwaiter().GetResult();
            var tenant = await _tenantRepository.GetByIdAsync(tenantId);
            var currencySymbol = _generalParameterRepository.GetQueryable().Where(gp => gp.Key == "Currency").FirstOrDefault()?.Value ?? "$";
            if (tenant?.Name is not null) await FCM(createdPromotion.Id, tenant.Name, description, startDate, endDate, price, currencySymbol);

            // Devolver el DTO de la promoción creada
            return promotionDTO;
        }

        public async Task DeleteAsync(int promotionId, int productId, int tenantId)
        {
            var entity = await _promotionProductRepository
                .GetByIdAsync(promotionId, productId);
            if (entity != null && tenantId == null)
            {
                await _promotionProductRepository.DeleteAsync(entity.PromotionId, entity.ProductId);
            }
            else
            {
                await _promotionProductRepository.DeleteAsync(entity.PromotionId, entity.ProductId, tenantId);
            }
        }

        public async Task<bool> FCM(int promotionId, string tenantName, string description, DateTime startDate, DateTime endDate, int price, string currencySymbol)
        {
            // Crear el mensaje de notificación
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = "Nueva oferta flash!",
                    Body = "Clickea aqui para enterarte mas!",
                },
                Topic = $"ofertas_{tenantName}",
                Android = new AndroidConfig()
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification()
                    {
                        ChannelId = "default"
                    }
                },
                Data = new Dictionary<string, string>
                {
                    { "Description", $"{description}\nPor el modico precio de {currencySymbol} {price}.\nDesde el {startDate}, hasta el {endDate}" },
                }
            };

            try
            {
                // Enviar el mensaje a Firebase Cloud Messaging
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Successfully sent message to topic: {response}");
                return true;
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"Error sending message to topic: {ex.Message}");
                throw new Exception($"Error al enviar la notificación a los usuarios suscritos al topic");
            }
        }

        public async Task<PromotionDTO?> UpdatePromotion(int PromotionId, int tenantId, string description, DateTime startDate, DateTime endDate, IEnumerable<int> branch, IEnumerable<int> product, int price)
        {
            // obtener la promoción por ID
            var promotionDTO = await GetPrmotionById(PromotionId);
            if (promotionDTO == null)
                throw new Exception($"No existe una promoción con el ID {PromotionId}");
            // Mapear el DTO a la entidad
            var promotion = MapToPromotion(promotionDTO);

            // Actualizar los campos de la promoción
            promotion.TenantId = tenantId;
            promotion.Description = description;
            promotion.StartDate = startDate;
            promotion.EndDate = endDate;
            promotion.Price = price;
            // Actualizar la promoción en la base de datos
            await _promotionRepository.UpdateAsync(promotion);
            await _promotionRepository.SaveChangesAsync();

            // Actualizar los productos asociados a la promoción
            var promotionProducts = _promotionProductRepository.GetQueryable()
                .Where(pp => pp.PromotionId == PromotionId).ToList();

            // modificar los productos asociados a la promoción
            foreach (var pp in promotionProducts)
            {
                // _promotionProductRepository.DeleteAsync(pp.PromotionId).GetAwaiter().GetResult();
                _promotionProductRepository.DeleteAsync(pp.PromotionId, pp.ProductId).GetAwaiter().GetResult();
            }
            _promotionProductRepository.SaveChangesAsync().GetAwaiter().GetResult();
            // Asociar los nuevos productos a la promoción
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

            // Actualizar los branches asociados a la promoción
            var promotionBranches = _promotionBranchRepository.GetQueryable()
                .Where(pb => pb.PromotionId == PromotionId).ToList();
            // modificar los branches asociados a la promoción
            foreach (var pb in promotionBranches)
            {
                _promotionBranchRepository.DeleteAsync(pb.PromotionId, pb.BranchId, pb.TenantId).GetAwaiter().GetResult();
            }
            _promotionBranchRepository.SaveChangesAsync().GetAwaiter().GetResult();
            // Asociar los nuevos branches a la promoción
            foreach (var bId in branch)
            {
                var promotionBranch = new DAO.Models.Central.PromotionBranch
                {
                    PromotionId = promotion.Id,
                    BranchId = bId,
                    TenantId = tenantId
                };
                _promotionBranchRepository.AddAsync(promotionBranch).GetAwaiter().GetResult();
            }
            _promotionBranchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Devolver el DTO de la promoción actualizada
            return GetPromotionDTO(promotion);
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
            var promotionBranch = _promotionBranchRepository.GetQueryable().Where(pb => pb.BranchId == branchId && pb.PromotionId == promotionId).FirstOrDefault();

            if (promotionBranch == null)
                throw new Exception($"No existe una promoción con el ID {promotionId} para la sucursal {branchId}");

            // Obtener PromotionProduct
            var promotionProduct = _promotionProductRepository.GetQueryable().Where(pp => pp.PromotionId == promotionId)
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
            // Validaciones previas - verificar existencia de productos
            verifyProductList(product);

            var promotion = new DAO.Models.Central.Promotion
            {
                TenantId = tenantId,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                Price = price
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
            var promotionBranch = new DAO.Models.Central.PromotionBranch
            {
                PromotionId = promotion.Id,
                BranchId = branchId,
                TenantId = tenantId
            };
            _promotionBranchRepository.AddAsync(promotionBranch).GetAwaiter().GetResult();

            _promotionBranchRepository.SaveChangesAsync().GetAwaiter().GetResult();

            // Devolver el DTO de la promoción creada
            return Task.FromResult(promotionDTO);
        }

        public PromotionExtendedDTO[] GetBranchPromotionList(int tenantId, int branchId)
      {
            // Obtener la lista de promociones del repositorio filtrando por TenantId
            var promotions = _promotionRepository.GetQueryable().Where(p => p.TenantId == tenantId);
            // Filtrar las promociones que están asociadas a la sucursal específica
            promotions = promotions.Where(p => _promotionBranchRepository.GetQueryable()
                .Any(pb => pb.PromotionId == p.Id && pb.BranchId == branchId && pb.TenantId == tenantId));

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
                .Where(t => t.Branch.TenantId == tenantIdInt)
                .CountAsync();

            // Contar promociones por tipo (tenant o branch) para este tenant
            var tenantPromotions = await _dbContext.Set<DAO.Models.Central.Promotion>()
                .Where(p => p.TenantId == tenantIdInt)
                .CountAsync();

            var branchPromotions = await _dbContext.Set<DAO.Models.Central.Promotion>()
                .Where(p => p.TenantId == tenantIdInt)
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
