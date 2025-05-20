using Microsoft.EntityFrameworkCore;
using ServiPuntosUy.Models.DAO;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DAO.Data.Central;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
});

// Configurar la conexión a la base de datos
builder.Services.AddDbContext<CentralDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CentralConnection")));

// Register DbContext as base class for generic repository
builder.Services.AddScoped<DbContext>(provider => provider.GetService<CentralDbContext>());

// Registrar repositorios
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Registrar servicios
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ILoginService, LoginService>();

// Registra la política CORS (antes de Build)
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaPolicy", policy =>
    {
        policy
          .WithOrigins("http://localhost:5173")
          .AllowAnyHeader()
          .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiPuntosUY API V1");
    c.RoutePrefix = "swagger";
});

app.UseRouting();
app.UseCors("SpaPolicy");
app.UseAuthorization();
app.MapControllers();
app.UseAuthentication();

app.Run();
