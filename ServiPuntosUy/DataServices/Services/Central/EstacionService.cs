using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Models.DAO;
using System.Linq.Expressions;

namespace ServiPuntosUy.DataServices.Services.Central;

public class EstacionService : IEstacionService
{
    private readonly IGenericRepository<Estacion> _estacionRepository;
    private readonly ITenantService _tenantService;

    public EstacionService(IGenericRepository<Estacion> estacionRepository, ITenantService tenantService)
    {
        _estacionRepository = estacionRepository;
        _tenantService = tenantService;
    }

    // Métodos de la Estación

    public EstacionDTO GetEstacionDTO(Estacion estacion) {
        return new EstacionDTO {
            Id = estacion.Id,
            Latitud = estacion.Latitud,
            Longitud = estacion.Longitud,
            TenantId = estacion.Tenant.Id,
            Tenant = _tenantService.GetTenantDTO(estacion.Tenant),
        };
    }

    public EstacionDTO GetEstacionById(int id) {
      var estacion = _estacionRepository.GetQueryable().FirstOrDefault(e => e.Id == id);
      return estacion != null ? GetEstacionDTO(estacion) : null;
    }

    public EstacionDTO CreateEstacion(string latitud, string longitud, string tenantId) {
      var tenant = _tenantService.GetTenantObjectByTenantId(tenantId);

      if (tenant == null) {
        throw new ArgumentException("No existe un tenant con el id " + tenantId);
      }

      var estacion = new Estacion {
        Latitud = latitud,
        Longitud = longitud,
        TenantId = tenant.Id,
        Tenant = tenant,
      };

      _tenantService.AddEstacionToTenant(tenantId, estacion);

      var createdEstacion = _estacionRepository.AddAsync(estacion).GetAwaiter().GetResult();
      _estacionRepository.SaveChangesAsync().GetAwaiter().GetResult();

      return GetEstacionDTO(createdEstacion);
    }
}
