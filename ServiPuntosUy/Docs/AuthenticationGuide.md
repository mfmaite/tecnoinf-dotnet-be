# Guía de Autenticación

## Autenticación JWT

ServiPuntos.uy utiliza JSON Web Tokens (JWT) para la autenticación de usuarios. Todos los endpoints de la API (excepto los de login y registro) requieren un token JWT válido para ser accedidos.

## Flujo de Autenticación

1. El usuario se autentica mediante el endpoint `/api/auth/login` proporcionando sus credenciales (email y contraseña).
2. Si las credenciales son válidas, el servidor devuelve un token JWT.
3. El cliente debe incluir este token en el encabezado `Authorization` de todas las solicitudes posteriores, con el formato `Bearer {token}`.
4. El servidor valida el token en cada solicitud y, si es válido, procesa la solicitud.
5. Si el token no es válido o ha expirado, el servidor devuelve un error 401 Unauthorized.

## Estructura del Token JWT

El token JWT contiene la siguiente información:

- **Subject**: ID del usuario
- **Name**: Nombre del usuario
- **Email**: Email del usuario
- **UserType**: Tipo de usuario (Central, Tenant, Branch, EndUser)
- **TenantId**: ID del tenant (solo para usuarios de tipo Tenant, Branch y EndUser)
- **BranchId**: ID de la estación (solo para usuarios de tipo Branch)
- **Issuer**: Emisor del token (ServiPuntos.uy)
- **Audience**: Audiencia del token (ServiPuntos.uy)
- **Expiration**: Fecha de expiración del token (8 horas después de la emisión)

## Ejemplo de Uso

### Autenticación

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@servipuntos.uy",
  "password": "password123"
}
```

Respuesta:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "name": "Admin",
    "email": "admin@servipuntos.uy",
    "userType": "Central",
    "tenantId": null
  }
}
```

### Uso del Token

```http
GET /api/tenant/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Pruebas con Swagger

Para probar la API con Swagger, sigue estos pasos:

1. Accede a la documentación de Swagger en `http://localhost:5162/swagger`.
2. Autentica al usuario mediante el endpoint `/api/auth/login`.
3. Copia el token JWT devuelto en la respuesta.
4. Haz clic en el botón "Authorize" en la parte superior de la página de Swagger.
5. En el campo "Value", ingresa `Bearer {token}` (reemplaza `{token}` con el token JWT copiado).
6. Haz clic en "Authorize" para guardar el token.
7. Ahora puedes probar cualquier endpoint de la API con el token JWT.

## Configuración

La configuración de JWT se encuentra en el archivo `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "clave-secreta-muy-larga-y-segura",
    "Issuer": "ServiPuntos.uy",
    "Audience": "ServiPuntos.uy"
  }
}
```

> ⚠️ **Importante**: La clave secreta debe ser lo suficientemente larga y segura para garantizar la seguridad de los tokens JWT. En producción, esta clave debe ser almacenada de forma segura y no debe ser incluida en el código fuente.

## Middleware de Autenticación

El middleware de autenticación JWT (`JwtAuthenticationMiddleware`) se encarga de validar el token JWT en cada solicitud. Este middleware está configurado para excluir ciertas rutas de la autenticación, como las de login, registro y Swagger.

```csharp
// Rutas excluidas de la autenticación JWT
_excludedPaths = new List<string>
{
    "/api/auth/login",
    "/swagger",
    "/api/auth/register",
    "/api/health"
};
```

Si necesitas excluir más rutas de la autenticación, puedes agregar sus paths a esta lista en el archivo `JwtAuthenticationMiddleware.cs`.
