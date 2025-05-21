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
                    TenantId = "central" // El administrador central no pertenece a ningún tenant específico
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
                TenantId = "central" // El administrador central no pertenece a ningún tenant específico
            }
        );
    }
}
