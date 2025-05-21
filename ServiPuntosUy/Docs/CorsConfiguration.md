# Configuración de CORS en ServiPuntosUy

Este documento explica la configuración de CORS (Cross-Origin Resource Sharing) en ServiPuntosUy, tanto para desarrollo como para producción.

## Configuración Actual (Desarrollo)

Actualmente, la API está configurada para permitir solicitudes de cualquier origen (CORS abierto) para facilitar el desarrollo. Esta configuración se encuentra en `Program.cs`:

```csharp
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
```

Y el middleware CORS se habilita con:

```csharp
// Habilitar CORS
app.UseCors("AllowAll");
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