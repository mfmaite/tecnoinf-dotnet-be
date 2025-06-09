using System;
using ServiceReference;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiPuntosUy.Models.DAO;
using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Enums;
using ServiPuntosUy.DataServices.Services.Tenant;
using ServiPuntosUy.DAO.Models.Central;



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
}
