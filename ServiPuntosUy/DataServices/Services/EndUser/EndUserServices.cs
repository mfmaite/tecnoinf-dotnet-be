using System;
using ServiceReference;
using System.ServiceModel;
using System.Threading.Tasks;
using ServiPuntosUy.Models.DAO;
using Microsoft.EntityFrameworkCore;

namespace ServiPuntosUy.DataServices.Services.EndUser
{
    /// <summary>
    /// Implementación del servicio de lealtad para el usuario final
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

        // Implementar los métodos de la interfaz ILoyaltyService
        // Esta es una implementación básica para el scaffold
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
    public class ProductService : IProductService
    {
        private readonly DbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public ProductService(DbContext dbContext, IConfiguration configuration, string tenantId)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _tenantId = tenantId;
        }

        // Implementar los métodos de la interfaz IProductService
        // Esta es una implementación básica para el scaffold
    }


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

        public async Task<string> VerificarIdentidad(int userId, string nroDoc)
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
                Console.WriteLine($"Fecha de nacimiento: {persona.FechaNacimiento}");
                return persona.FechaNacimiento;
            }
            else
            {
                throw new Exception("Error al verificar la identidad del usuario");
            }
        }
    }
}
