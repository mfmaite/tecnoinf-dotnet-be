using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.Models.DAO;
using ServiPuntosUy.DAO.Data.Central;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Middlewares;
using ServiPuntosUy.Utils;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using DotNetEnv;

// Cargar variables de entorno
var envPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}
else
{
    Console.WriteLine("Warning: .env file not found at: " + envPath);
}

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar CORS utilizando el utilitario
builder.Services.AddCorsConfiguration(builder.Environment);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ServiPuntosUY API",
        Version = "v1",
        Description = "API para la gestión de ServiPuntosUY"
    });

    // Configurar la seguridad JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Configure Firebase Admin SDK
var serviceAccountKeyPath = builder.Configuration.GetValue<string>("Firebase:ServiceAccountKeyPath");
if (string.IsNullOrEmpty(serviceAccountKeyPath))
{
    throw new InvalidOperationException("Firebase:ServiceAccountKeyPath is not configured in appsettings.json.");
}

try
{
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
    });
    Console.WriteLine("Firebase Admin SDK initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error initializing Firebase Admin SDK: {ex.Message}");
    // Consider logging the full exception for debugging
    throw; // Re-throw to prevent the application from starting if initialization fails
}




// Configurar la conexión a la base de datos
builder.Services.AddDbContext<CentralDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CentralConnection")));

// Register DbContext as base class for generic repository
builder.Services.AddScoped<DbContext>(provider => provider.GetService<CentralDbContext>());

// Registrar repositorios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


// Registrar servicios globales que no dependen del tenant/usuario
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<ITenantResolver>(provider =>
    new TenantResolver(
        provider.GetRequiredService<IConfiguration>(),
        provider.GetRequiredService<CentralDbContext>(),
        provider.GetRequiredService<IHostEnvironment>()
    ));
builder.Services.AddScoped<IServiceFactory, ServiceFactory>();

// Registrar el servicio de email
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiPuntosUY API V1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();

// Habilitar CORS utilizando el utilitario
app.UseCorsConfiguration();

// Configuración del middleware
app.UseJwtAuthentication(); // Middleware personalizado para autenticación JWT (chequea token)
app.UseRequestContent(); // Middleware para identificar tenant y tipo de usuario
app.UseApiResponseWrapper(); // Middleware para envolver respuestas en ApiResponse
app.UseAuthorization();

app.MapControllers();

app.Run();
