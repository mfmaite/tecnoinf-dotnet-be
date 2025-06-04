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

    public DbSet<Branch> Branches { get; set; }
    public DbSet<FuelPrices> FuelPrices { get; set; }
    public DbSet<LoyaltyConfig> LoyaltyConfigs { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductStock> ProductStocks { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Redemption> Redemptions { get; set; }
    public DbSet<SatisfactionSurvey> SatisfactionSurveys { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<ServiceAvailability> ServiceAvailabilities { get; set; }
    public DbSet<TenantUI> TenantUIs { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
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
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductStock>()
            .HasOne(ps => ps.Product)
            .WithMany()
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProductStock>()
            .HasOne(ps => ps.Branch)
            .WithMany()
            .HasForeignKey(ps => ps.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceAvailability>()
            .HasOne(sa => sa.Branch)
            .WithMany()
            .HasForeignKey(sa => sa.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ServiceAvailability>()
            .HasOne(sa => sa.Service)
            .WithMany()
            .HasForeignKey(sa => sa.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar FK TenantId en FuelPrices para evitar cascada (multiple cascade paths)
        modelBuilder.Entity<FuelPrices>()
            .HasOne(f => f.Tenant)
            .WithMany()
            .HasForeignKey(f => f.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed para el primer usuario administrador
        SeedAdminUser(modelBuilder);
    }


    private void SeedAdminUser(ModelBuilder modelBuilder)
    {
        // Crear Tenant con Id=1 siempre
        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = -1,
                Name = "ancap"
            }
        );

        // Si estamos en tiempo de diseño (migraciones), usamos valores por defecto
        if (_configuration == null)
        {
            // Valores por defecto para migraciones

            // Crea admin central
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = -1,
                    Name = "Admin Central",
                    Email = "admin@servipuntos.uy",
                    Password = "AdminPassword", // En producción, esto sería un hash
                    PasswordSalt = "DefaultSalt", // En producción, esto sería un salt real
                    Role = UserType.Central,
                    LastLoginDate = DateTime.UtcNow // Establecer fecha de último login
                }
            );

            // Crea Admin Tenant
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = -2,
                    Name = "Admin Tenant",
                    Email = "admintenant@servipuntos.uy",
                    Password = "AdminPassword", // En producción, esto sería un hash
                    PasswordSalt = "DefaultSalt", // En producción, esto sería un salt real
                    Role = UserType.Tenant,
                    LastLoginDate = DateTime.UtcNow // Establecer fecha de último login
                }
            );
            return;
        }

        // En tiempo de ejecución, usamos AuthLogic para generar hash y salt
        var authLogic = new AuthLogic(_configuration);
        string salt;
        var passwordHash = authLogic.HashPassword("Admin123!", out salt);

        // Crea Admin Central
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = -1,
                Name = "Admin Central",
                Email = "admin@servipuntos.uy",
                Password = passwordHash,
                PasswordSalt = salt,
                Role = UserType.Central,
                LastLoginDate = DateTime.UtcNow
            }
        );

        // Crea admin tenant
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = -2,
                Name = "Admin Tenant",
                Email = "admintenant@servipuntos.uy",
                Password = passwordHash,
                PasswordSalt = salt,
                Role = UserType.Tenant,
                TenantId = -1,
                LastLoginDate = DateTime.UtcNow
            }
        );
        // Crea admin branch
                modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 3,
                Name = "Admin branch",
                Email = "adminAncap1@servipuntos.uy",
                Password = passwordHash,
                PasswordSalt = salt,
                Role = UserType.Branch,
                TenantId = 1,
                LastLoginDate = DateTime.UtcNow,
                BranchId = 1 // Asignar a la primera estación por defecto
            }
        );
    }
    
}
