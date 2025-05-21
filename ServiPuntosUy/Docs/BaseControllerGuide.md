# Guía del BaseController en ServiPuntosUy

Este documento explica el propósito y funcionamiento del `BaseController` en ServiPuntosUy, así como las mejores prácticas para utilizarlo en controladores derivados.

## Introducción

El `BaseController` es una clase base personalizada que extiende la clase `Controller` de ASP.NET Core MVC. Su propósito principal es proporcionar funcionalidad común a todos los controladores de la aplicación, facilitando el acceso a:

- Información del tenant actual
- Tipo de usuario actual
- Servicios específicos según el tenant y tipo de usuario
- Métodos de ayuda para trabajar con tokens JWT

Utilizar el `BaseController` como clase base para los controladores de la aplicación permite:

1. **Reducir la duplicación de código**: La funcionalidad común se implementa una sola vez en el `BaseController`.
2. **Mantener la consistencia**: Todos los controladores derivados acceden a la información del tenant y tipo de usuario de la misma manera.
3. **Simplificar el acceso a servicios**: Los servicios específicos según el tenant y tipo de usuario están disponibles como propiedades protegidas.
4. **Mejorar la seguridad**: Incluye métodos para verificar el acceso a los tenants.

## Propiedades y Métodos Heredados

### Propiedades para Acceder al Contexto

El `BaseController` proporciona las siguientes propiedades para acceder a la información del contexto:

```csharp
// Obtiene el tenant actual del contexto HTTP
protected string CurrentTenant => HttpContext.Items["CurrentTenant"] as string;

// Obtiene el tipo de usuario actual del contexto HTTP
protected UserType UserType => HttpContext.Items["UserType"] is UserType userType ? userType : UserType.EndUser;

// Obtiene el ID de la estación actual del contexto HTTP (solo para administradores de estación)
protected int? BranchId => HttpContext.Items["BranchId"] as int?;
```

Estas propiedades permiten a los controladores derivados acceder fácilmente a la información del tenant y tipo de usuario que ha sido establecida por el middleware `RequestContentMiddleware`.

### Métodos para Trabajar con Tokens JWT

El `BaseController` incluye métodos para extraer información de los tokens JWT:

```csharp
// Obtiene el usuario actual del token JWT
protected UserDTO ObtainUserFromToken();

// Obtiene el tenant actual del token JWT
protected string ObtainTenantFromToken();

// Obtiene el tipo de usuario del token JWT
protected UserType ObtainUserTypeFromToken();

// Obtiene el ID de la estación del token JWT (solo para administradores de estación)
protected int? ObtainBranchIdFromToken();
```

Estos métodos permiten a los controladores derivados acceder a la información contenida en el token JWT sin tener que implementar la lógica de extracción de claims en cada controlador.

### Métodos de Autorización

El `BaseController` incluye métodos para verificar el acceso a los tenants:

```csharp
// Verifica si el usuario actual tiene acceso al tenant especificado
protected bool HasAccessToTenant(string tenantId);
```

Este método permite a los controladores derivados verificar si el usuario actual tiene acceso a un tenant específico, implementando la lógica de autorización basada en el tipo de usuario.

### Acceso a Servicios

El `BaseController` proporciona propiedades para acceder a los servicios específicos según el tenant y tipo de usuario:

```csharp
// Obtiene el servicio de autenticación
protected IAuthService AuthService => _serviceFactory.GetService<IAuthService>();

// Obtiene el servicio de tenant
protected ICentralTenantService TenantService => _serviceFactory.GetService<ICentralTenantService>();

// ... y muchos más servicios
```

Estas propiedades permiten a los controladores derivados acceder a los servicios específicos según el tenant y tipo de usuario sin tener que inyectarlos en cada controlador.

## Cómo Utilizar el BaseController

Para utilizar el `BaseController` como clase base para un controlador, simplemente herede de él en lugar de `Controller` o `ControllerBase`:

```csharp
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;

namespace ServiPuntosUy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyController : BaseController
    {
        public MyController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Acceder a la información del tenant y tipo de usuario
            var tenant = CurrentTenant;
            var userType = UserType;
            
            // Verificar el acceso al tenant
            if (!HasAccessToTenant(tenant))
            {
                return Forbid();
            }
            
            // Utilizar servicios específicos según el tenant y tipo de usuario
            var users = UserService.GetUsers();
            
            return Ok(users);
        }
    }
}
```

## Ejemplo Práctico: Controlador de Promociones

A continuación se muestra un ejemplo de un controlador de promociones que hereda de `BaseController`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DTO;

namespace ServiPuntosUy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PromotionController : BaseController
    {
        public PromotionController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        [HttpGet]
        public IActionResult GetPromotions()
        {
            try
            {
                // Utilizar el servicio de promociones específico según el tenant y tipo de usuario
                var promotions = PromotionService.GetPromotions();
                
                return Ok(promotions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetPromotion(int id)
        {
            try
            {
                // Utilizar el servicio de promociones específico según el tenant y tipo de usuario
                var promotion = PromotionService.GetPromotionById(id);
                
                if (promotion == null)
                    return NotFound($"Promotion with ID {id} not found");
                
                // Verificar el acceso al tenant de la promoción
                if (!HasAccessToTenant(promotion.TenantId))
                {
                    return Forbid();
                }
                
                return Ok(promotion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreatePromotion([FromBody] PromotionDTO promotion)
        {
            try
            {
                // Verificar el acceso al tenant de la promoción
                if (!HasAccessToTenant(promotion.TenantId))
                {
                    return Forbid();
                }
                
                // Utilizar el servicio de promociones específico según el tenant y tipo de usuario
                var newPromotion = PromotionService.CreatePromotion(promotion);
                
                return Ok(newPromotion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
```

## Mejores Prácticas

### Cuándo Usar el BaseController

El `BaseController` es especialmente útil en los siguientes casos:

1. **Controladores que necesitan acceder a la información del tenant y tipo de usuario**: Si el controlador necesita conocer el tenant actual o el tipo de usuario, heredar de `BaseController` simplifica este acceso.

2. **Controladores que utilizan servicios específicos según el tenant y tipo de usuario**: Si el controlador necesita utilizar servicios que varían según el tenant y tipo de usuario, heredar de `BaseController` proporciona acceso a estos servicios.

3. **Controladores que implementan lógica de autorización basada en tenants**: Si el controlador necesita verificar si el usuario tiene acceso a un tenant específico, heredar de `BaseController` proporciona métodos para esta verificación.

### Cuándo No Usar el BaseController

En algunos casos, puede ser preferible no utilizar el `BaseController`:

1. **Controladores simples que no necesitan acceder a la información del tenant o tipo de usuario**: Si el controlador no necesita esta información, heredar de `Controller` o `ControllerBase` puede ser suficiente.

2. **Controladores que no utilizan servicios específicos según el tenant y tipo de usuario**: Si el controlador utiliza servicios que no varían según el tenant y tipo de usuario, no es necesario heredar de `BaseController`.

### Manejo de Excepciones

Es importante manejar adecuadamente las excepciones en los controladores derivados de `BaseController`. Aunque el `BaseController` proporciona métodos para trabajar con tokens JWT y acceder a servicios, no implementa manejo de excepciones global.

Se recomienda utilizar bloques try-catch en los métodos de acción para capturar y manejar las excepciones de manera adecuada:

```csharp
[HttpGet]
public IActionResult Get()
{
    try
    {
        // Código que puede lanzar excepciones
        var result = SomeService.DoSomething();
        return Ok(result);
    }
    catch (Exception ex)
    {
        // Manejar la excepción
        return BadRequest(ex.Message);
    }
}
```