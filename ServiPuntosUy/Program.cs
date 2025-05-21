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

// Configurar CORS para desarrollo (permitir cualquier origen)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

// Configurar la conexión a la base de datos
builder.Services.AddDbContext<CentralDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CentralConnection")));

// Register DbContext as base class for generic repository
builder.Services.AddScoped<DbContext>(provider => provider.GetService<CentralDbContext>());

// Registrar repositorios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


// Registrar servicios globales que no dependen del tenant/usuario
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();
builder.Services.AddScoped<IServiceFactory, ServiceFactory>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiPuntosUY API V1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();

// Habilitar CORS
app.UseCors("AllowAll");

// Configuración del middleware
app.UseMiddleware<RequestContentMiddleware>();
app.UseMiddleware<JwtAuthenticationMiddleware>(); // Middleware personalizado para autenticación JWT (chequea token)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
