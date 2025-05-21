using System.Linq.Expressions;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;
using System.Text.RegularExpressions;
namespace ServiPuntosUy.DataServices.Services.Central;

public class TenantService : ITenantService
{
    private readonly IGenericRepository<Tenant> _tenantRepository;

    public TenantService(IGenericRepository<Tenant> tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    // Métodos del Tenant

    public TenantDTO GetTenantDTO(Tenant tenant) {
      return new TenantDTO {
        Id = tenant.Id,
        Name = tenant.Name,
        TenantId = tenant.TenantId,
      };
    }

    public Tenant GetTenantObjectByTenantId(string tenantId)
    {
        var condicion = new List<Expression<Func<Tenant, bool>>>
        {
            tenant => tenant.TenantId == tenantId
        };

        var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.TenantId == tenantId);
        return tenant;
    }

    public TenantDTO GetTenantDTOByTenantId(string tenantId)
    {
        return GetTenantDTO(GetTenantObjectByTenantId(tenantId));
    }

    public TenantDTO CreateTenant(string name, string tenantId) {
      // Validar que tenantId contenga solo letras A-Z (mayus y minus)
      if (!Regex.IsMatch(tenantId, @"^[a-zA-Z]+$"))
      {
          throw new ArgumentException("El TenantId solo puede contener letras mayúsculas de la A a la Z.");
      }

      // Verificar que el tenantId no exista
      bool tenantExists = _tenantRepository.GetQueryable().Any(t => t.TenantId == tenantId);
      if (tenantExists)
      {
          throw new ArgumentException($"El TenantId '{tenantId}' ya existe. Debe ser único.");
      }


      var newTenant = new Tenant {
        Name = name,
        TenantId = tenantId,
      };

        var createdTenant = _tenantRepository.AddAsync(newTenant).GetAwaiter().GetResult();
        _tenantRepository.SaveChangesAsync().GetAwaiter().GetResult();

        return GetTenantDTO(createdTenant);
    }
}
