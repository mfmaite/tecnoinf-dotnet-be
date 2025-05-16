using System.Linq.Expressions;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;

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
        DatabaseName = tenant.DatabaseName,
        ConnectionString = tenant.ConnectionString,
        User = tenant.User,
      };
    }

    public TenantDTO GetTenantById(int id)
    {
        var condicion = new List<Expression<Func<Tenant, bool>>>
        {
            tenant => tenant.Id == id
        };

        var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.Id == id);
        return tenant != null ? GetTenantDTO(tenant) : null;
    }

    public TenantDTO CreateTenant(string name, string databaseName, string connectionString, string user, string password) {
      var newTenant = new Tenant {
        Name = name,
        DatabaseName = databaseName,
        ConnectionString = connectionString,
        User = user,
        Password = password,
      };

        var createdTenant = _tenantRepository.AddAsync(newTenant).GetAwaiter().GetResult();
        _tenantRepository.SaveChangesAsync().GetAwaiter().GetResult();

        return GetTenantDTO(createdTenant);
    }
}
