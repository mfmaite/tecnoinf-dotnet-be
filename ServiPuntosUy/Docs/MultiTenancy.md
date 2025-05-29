# Arquitectura Multi-Tenant de ServiPuntosUy

Este documento describe la arquitectura multi-tenant implementada en ServiPuntosUy, explicando cómo los diferentes componentes trabajan juntos para proporcionar una experiencia personalizada a cada cadena de estaciones de servicio (tenant).

## Visión General

ServiPuntosUy utiliza un enfoque de **discriminador por tenant** para implementar el multi-tenancy. Esto significa que:

1. Todos los datos se almacenan en una única base de datos
2. Cada tabla relacionada con los tenants incluye una columna `TenantId`
3. Las consultas se filtran automáticamente por el tenant actual
4. Los servicios se configuran dinámicamente según el tenant y tipo de usuario

## Componentes Principales

### 1. TenantResolver

El `TenantResolver` es responsable de identificar el tenant y el tipo de usuario a partir del contexto HTTP. Implementa la interfaz `ITenantResolver` con los siguientes métodos:

- `ResolveTenantId(HttpContext)`: Determina el ID del tenant basado en (en orden de prioridad):
  - Token JWT (claim "tenantId") si el usuario está autenticado
  - Host de la solicitud (patrón {tenant-name}.app.servipuntos.uy)
  - Header X-Tenant-Name (para usuarios móviles)
  - Query parameter "tenant" (para desarrollo)

- `ResolveUserType(HttpContext)`: Determina el tipo de usuario basado en (en orden de prioridad):
  - Token JWT (claim "userType") si el usuario está autenticado
  - Host de la solicitud (solo identifica EndUser para patrón {tenant-name}.app.servipuntos.uy)
  - Query parameter "userType" (para desarrollo)

- `ResolveBranchId(HttpContext)`: Determina el ID de la estación para administradores de estación basado en:
  - Token JWT (claim "branchId")
  - Ruta de la URL
  - Query parameter "branchId"

### 2. RequestContentMiddleware

El middleware `RequestContentMiddleware` se ejecuta al inicio de cada solicitud y realiza las siguientes tareas:

1. Identifica el tenant usando `TenantResolver.ResolveTenantId`
2. Identifica el tipo de usuario usando `TenantResolver.ResolveUserType`
3. Configura los servicios según el tenant y tipo de usuario usando `ServiceFactory.ConfigureServices`
4. Almacena el tenant y tipo de usuario en `HttpContext.Items` para uso posterior
5. Para administradores de estación, resuelve y almacena el ID de la estación

### 3. ServiceFactory

El `ServiceFactory` es responsable de configurar y proporcionar los servicios adecuados según el tenant y tipo de usuario. Implementa la interfaz `IServiceFactory` con los siguientes métodos:

- `GetService<T>()`: Obtiene un servicio del tipo especificado del proveedor de servicios configurado para el tenant actual
- `ConfigureServices(tenantId, userType, httpContext)`: Configura los servicios según el tenant y tipo de usuario

El `ServiceFactory` utiliza un enfoque de "scope" para cada tenant/usuario, creando un nuevo `IServiceProvider` con los servicios adecuados para cada combinación de tenant y tipo de usuario.

Además, el `ServiceFactory` verifica si el usuario está autenticado:
- Si el usuario no está autenticado, solo configura servicios comunes necesarios para el login
- Si el usuario está autenticado, configura los servicios completos según el tenant y tipo de usuario

### 4. TenantAccessor

El `TenantAccessor` proporciona acceso al tenant actual desde cualquier parte de la aplicación. Implementa la interfaz `ITenantAccessor` con el siguiente método:

- `GetCurrentTenantId()`: Obtiene el ID del tenant actual

## Flujo de Ejecución

1. Una solicitud llega al servidor
2. El `RequestContentMiddleware` intercepta la solicitud
3. El `JwtAuthenticationMiddleware` verifica si el token JWT es válido:
   - Si es válido, establece `User.Identity.IsAuthenticated = true` y crea un `ClaimsPrincipal` con los claims del token
   - Si no es válido, devuelve un 401 Unauthorized
4. El middleware utiliza el `TenantResolver` para identificar el tenant y tipo de usuario
5. El `ServiceFactory` verifica si el usuario está autenticado:
   - Si no está autenticado, solo configura servicios comunes para login
   - Si está autenticado, configura los servicios completos según el tenant y tipo de usuario
6. El middleware almacena el tenant y tipo de usuario en el contexto HTTP (si el usuario está autenticado)
7. La solicitud continúa su procesamiento normal
8. Los controladores y servicios utilizan el `ServiceFactory` para obtener los servicios adecuados
9. Los servicios utilizan el `TenantAccessor` para obtener el tenant actual cuando sea necesario

## Estructura de Dominios

La aplicación utiliza la siguiente estructura de dominios para diferenciar entre los diferentes tipos de usuarios:

- **admin.servipuntos.uy**: Panel de administración unificado (para Central, Tenant y Branch admins)
- **{tenant-name}.app.servipuntos.uy**: Usuario final (EndUser) con tenant específico

Para los administradores (Central, Tenant y Branch), el tipo de usuario se determina a partir del JWT después del login, no de la URL. Esto permite tener un único panel de administración para todos los tipos de administradores.

## Modelo de Datos

El modelo de datos utiliza un enfoque de discriminador por tenant, donde cada entidad relacionada con los tenants incluye una columna `TenantId`. Las entidades base implementan una clase base `BaseEntity` que incluye esta columna.

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public string TenantId { get; set; }
}
```

El `DbContext` aplica automáticamente un filtro global para todas las entidades que heredan de `BaseEntity`, asegurando que las consultas solo devuelvan datos del tenant actual.

## Implementación de Servicios

Los servicios se implementan en diferentes namespaces según el tipo de usuario:

- `ServiPuntosUy.DataServices.Services.Central`: Servicios para el administrador central
- `ServiPuntosUy.DataServices.Services.Tenant`: Servicios para el administrador de tenant
- `ServiPuntosUy.DataServices.Services.Branch`: Servicios para el administrador de estación
- `ServiPuntosUy.DataServices.Services.EndUser`: Servicios para el usuario final

Cada implementación de servicio recibe el tenant actual y, en el caso de los servicios de estación, también el ID de la estación.
