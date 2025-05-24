using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DAO.Data.Central;

public class CentralDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public CentralDbContext(DbContextOptions<CentralDbContext> options, IConfiguration configuration = null) : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones de las entidades
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Configurar FK TenantId para evitar cascada (multiple cascade paths)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Tenant)
            .WithMany() // o .WithMany(t => t.Users) si Tenant tiene la colección
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed para el primer usuario administrador
        SeedAdminUser(modelBuilder);
    }


    private void SeedAdminUser(ModelBuilder modelBuilder)
    {
        // Si estamos en tiempo de diseño (migraciones), usamos valores por defecto
        if (_configuration == null)
        {
            // Valores por defecto para migraciones
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin Central",
                    Email = "admin@servipuntos.uy",
                    Password = "AdminPassword", // En producción, esto sería un hash
                    PasswordSalt = "DefaultSalt", // En producción, esto sería un salt real
                    Role = UserType.Central,
                }
            );
            return;
        }

        // En tiempo de ejecución, usamos AuthLogic para generar hash y salt
        var authLogic = new AuthLogic(_configuration);
        string salt;
        var passwordHash = authLogic.HashPassword("Admin123!", out salt);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Admin Central",
                Email = "admin@servipuntos.uy",
                Password = passwordHash,
                PasswordSalt = salt,
                Role = UserType.Central,
                TenantId = 1 // El administrador central no pertenece a ningún tenant específico
            }
        );
    }
}
