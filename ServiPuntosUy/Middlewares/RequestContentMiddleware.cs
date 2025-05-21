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

        public RequestContentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceFactory serviceFactory, ITenantResolver tenantResolver)
        {
            try
            {
                // 1. Identificar el tenant
                string tenantId = tenantResolver.ResolveTenantId(context);
                
                // 2. Identificar el tipo de usuario
                UserType userType = tenantResolver.ResolveUserType(context);
                
                // 3. Configurar los servicios según el tenant y tipo de usuario
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
                
                // 4. Agregar información del tenant y tipo de usuario al contexto para uso posterior
                context.Items["CurrentTenant"] = tenantId;
                context.Items["UserType"] = userType;
                
                // 5. Para administradores de estación, intentar resolver el ID de la estación
                if (userType == UserType.Branch)
                {
                    int? branchId = tenantResolver.ResolveBranchId(context);
                    if (branchId.HasValue)
                    {
                        context.Items["BranchId"] = branchId.Value;
                    }
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
