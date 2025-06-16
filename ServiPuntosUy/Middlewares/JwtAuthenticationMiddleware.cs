using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServiPuntosUy.DataServices.Services.CommonLogic;

namespace ServiPuntosUy.Middlewares
{
    /// <summary>
    /// Middleware para verificar la autenticación JWT en todas las solicitudes y establecer el usuario autenticado
    /// </summary>
    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtAuthenticationMiddleware> _logger;
        private readonly List<string> _excludedPaths;

        public JwtAuthenticationMiddleware(
            RequestDelegate next,
            IConfiguration configuration,
            ILogger<JwtAuthenticationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;

            // Rutas excluidas de la autenticación JWT
            _excludedPaths = new List<string>
            {
                "/api/auth/login",
                "/api/auth/signup",
                "/swagger",
                "/api/auth/register",
                "/api/health",
                "/api/tenantui/public", // Ruta pública para obtener la UI del tenant actual
                "/api/redemption/process", // Ruta para procesar canjes por QR
                "/api/public/tenant" // Ruta pública para listar tenants
            };
        }

        public async Task InvokeAsync(HttpContext context, IAuthLogic authLogic)
        {
            // Verificar si la ruta está excluida de la autenticación
            string path = context.Request.Path.Value.ToLower();

            if (_excludedPaths.Any(p => path.StartsWith(p)))
            {
                // Ruta excluida, continuar con el pipeline
                await _next(context);
                return;
            }

            // Obtener el token JWT del encabezado de autorización
            string authHeader = context.Request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                // No hay token JWT, devolver 401 Unauthorized
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "No se proporcionó un token de autenticación válido" });
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Validar el token JWT
                if (!authLogic.ValidateToken(token))
                {
                    // Token JWT inválido, devolver 401 Unauthorized
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new { error = "El token de autenticación proporcionado no es válido" });
                    return;
                }

                // Token JWT válido, crear un ClaimsPrincipal y asignarlo a HttpContext.User
                var claims = authLogic.GetTokenClaims(token);
                var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme, "userId", "userType");
                var principal = new ClaimsPrincipal(identity);

                // Establecer el usuario autenticado en el contexto
                context.User = principal;

                // Continuar con el pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Error al validar el token JWT, devolver 401 Unauthorized
                _logger.LogError(ex, "Error al validar el token JWT");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Error al validar el token de autenticación" });
            }
        }
    }

}
