using ServiPuntosUy.DataServices;
using ServiPuntosUy.DataServices.Services.CommonLogic;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.Middlewares
{
    /// <summary>
    /// Middleware para identificar el tenant y tipo de usuario, y configurar los servicios correspondientes
    /// </summary>
    public class RequestContentMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor del middleware que recibe el siguiente delegate en la cadena.
        /// </summary>
        /// <param name="next">Delegate que representa el siguiente middleware.</param>
        public RequestContentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método que procesa la petición HTTP.
        /// </summary>
        /// <param name="context">Contexto HTTP actual.</param>
        /// <param name="serviceFactory">Fábrica de servicios inyectada.</param>
        /// <param name="tenantResolver">Servicio para resolver el tenant.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        public async Task InvokeAsync(HttpContext context, IServiceFactory serviceFactory, ITenantResolver tenantResolver)
        {
            try
            {
                // 1. Identificar el tenant
                string tenantId = tenantResolver.ResolveTenantId(context);

                // 2. Identificar el tipo de usuario
                UserType userType = tenantResolver.ResolveUserType(context);

                // 3. Agregar información del tenant y tipo de usuario al contexto para uso posterior (antes de configurar servicios)
                context.Items["CurrentTenant"] = tenantId;
                context.Items["UserType"] = userType;

                // 4. Para administradores de estación, intentar resolver el ID de la estación
                if (userType == UserType.Branch)
                {
                    int? branchId = tenantResolver.ResolveBranchId(context);
                    if (branchId.HasValue)
                    {
                        context.Items["BranchId"] = branchId.Value;
                    }
                }

                // 5. Configurar los servicios según el tenant y tipo de usuario
                // Ahora el contexto ya tiene toda la información necesaria, incluido el branchId si es un administrador de estación
                try
                {
                    serviceFactory.ConfigureServices(tenantId, userType, context);
                }
                catch (Exception ex)
                {
                    // En desarrollo, manejar errores de configuración de servicios
                    if (context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsJsonAsync(new { error = $"Error configurando servicios: {ex.Message}" });
                        return;
                    }
                    throw; // En producción, propagar el error
                }

                // 6. Continuar con el pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Manejar errores generales del middleware
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = $"Error en el middleware: {ex.Message}" });
            }
        }
    }
}
