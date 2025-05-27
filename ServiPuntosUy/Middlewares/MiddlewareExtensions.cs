using Microsoft.AspNetCore.Builder;

namespace ServiPuntosUy.Middlewares
{
    /// <summary>
    /// Clase para registrar middlewares personalizados
    /// </summary>
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiResponseWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiResponseMiddleware>();
        }

        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtAuthenticationMiddleware>();
        }

        public static IApplicationBuilder UseRequestContent(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestContentMiddleware>();
        }
    }
}
