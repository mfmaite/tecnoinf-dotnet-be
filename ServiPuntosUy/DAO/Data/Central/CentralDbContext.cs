using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DAO.Data.Central;

public class CentralDbContext : DbContext
{
    public CentralDbContext(DbContextOptions<CentralDbContext> options) : base(options) {}

    public DbSet<CentralUser> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        // Aquí irán las configuraciones de las entidades
                // Inserta datos iniciales para la tabla Users
        modelBuilder.Entity<CentralUser>().HasData(
            new CentralUser
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@example.com",
                Password = "hashed_password", // Asegúrate de usar un hash real
            },
            new CentralUser
            {
                Id = 2,
                Name = "User",
                Email = "user@example.com",
                Password = "hashed_password", // Asegúrate de usar un hash real
            }
        );

    }

}
