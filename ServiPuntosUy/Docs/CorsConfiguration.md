# Configuración de CORS en ServiPuntosUy

Este documento explica la configuración de CORS (Cross-Origin Resource Sharing) en ServiPuntosUy, tanto para desarrollo como para producción.

## Configuración Actual

La configuración de CORS está encapsulada en la clase utilitaria `CorsConfigurationUtility` ubicada en `ServiPuntosUy/Utils/CorsConfigurationUtility.cs`. Esta clase proporciona métodos de extensión para configurar CORS de manera consistente en toda la aplicación.

### Uso en Program.cs

```csharp
// Configurar CORS utilizando el utilitario
builder.Services.AddCorsConfiguration(builder.Environment);

// Y más adelante en el código:
// Habilitar CORS utilizando el utilitario
app.UseCorsConfiguration();
```

### Implementación del Utilitario

```csharp
public static class CorsConfigurationUtility
{
    private const string DefaultCorsPolicyName = "CorsPolicy";

    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services, 
        IHostEnvironment environment,
        string policyName = DefaultCorsPolicyName)
    {
        if (environment.IsDevelopment())
        {
            // Development configuration
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });
        }
        else
        {
            // Production configuration
            services.AddCors(options =>
            {
                options.AddPolicy(policyName, policy =>
                {
                    policy.SetIsOriginAllowed(origin => {
                        // Validate that the origin ends with .servipuntos.me
                        return origin.EndsWith(".servipuntos.me");
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
        }

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(
        this IApplicationBuilder app,
        string policyName = DefaultCorsPolicyName)
    {
        return app.UseCors(policyName);
    }
}
```

## Consideraciones de Seguridad

La configuración actual es **solo para desarrollo** y no debe usarse en producción, ya que permite solicitudes desde cualquier origen, lo que podría exponer la API a ataques CSRF (Cross-Site Request Forgery) y otros problemas de seguridad.

## Configuración Recomendada para Producción

Antes de desplegar a producción, se debe modificar la configuración de CORS para restringir los orígenes permitidos. A continuación se muestra un ejemplo de configuración más segura:

```csharp
// Configurar CORS para producción (orígenes específicos)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins(
                "https://app.servipuntos.uy",
                "https://*.app.servipuntos.uy",
                "https://*.admin.servipuntos.uy",
                "https://admin.servipuntos.uy"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Si se necesitan cookies/autenticación
    });
});
```

Y el middleware CORS se habilitaría con:

```csharp
// Habilitar CORS
app.UseCors("ProductionPolicy");
```

## Configuración Basada en Entorno

Una mejor práctica es configurar CORS de manera diferente según el entorno (desarrollo vs. producción):

```csharp
if (app.Environment.IsDevelopment())
{
    // Configuración para desarrollo
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });
}
else
{
    // Configuración para producción
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.WithOrigins(
                    "https://app.servipuntos.uy",
                    "https://*.app.servipuntos.uy",
                    "https://*.admin.servipuntos.uy",
                    "https://admin.servipuntos.uy"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });
}

// Y luego en la configuración de la aplicación:
app.UseCors("CorsPolicy");
```

## Consideraciones Adicionales

### Comodines en Orígenes

La configuración `WithOrigins("https://*.app.servipuntos.uy")` no funcionará directamente, ya que ASP.NET Core no admite comodines en los orígenes. Para manejar subdominios, se pueden enumerar todos los orígenes permitidos o implementar una política CORS personalizada.

### Política CORS Personalizada para Subdominios

Para manejar subdominios de manera dinámica, se puede implementar una política CORS personalizada:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("SubdomainPolicy", policy =>
    {
        policy.SetIsOriginAllowedToAllowWildcardSubdomains()
              .WithOrigins("https://*.servipuntos.uy")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### Credenciales y Cookies

Si la API utiliza cookies o autenticación basada en credenciales, es importante incluir `.AllowCredentials()` en la política CORS. Sin embargo, esto no es compatible con `AllowAnyOrigin()`, por lo que se deben especificar los orígenes permitidos.
