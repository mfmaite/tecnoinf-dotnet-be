using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DAO.Data.Central;

public class CentralDbContext : DbContext
{
    public CentralDbContext(DbContextOptions<CentralDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        // Aquí irán las configuraciones de las entidades
    }
}
