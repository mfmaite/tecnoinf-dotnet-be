using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ServiPuntosUy.Utils
{
    /// <summary>
    /// Utility class for configuring CORS in the application
    /// </summary>
    public static class CorsConfigurationUtility
    {
        private const string DefaultCorsPolicyName = "CorsPolicy";

        /// <summary>
        /// Adds CORS configuration to the service collection based on the environment
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="environment">The hosting environment</param>
        /// <param name="policyName">The name of the CORS policy (default: "CorsPolicy")</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddCorsConfiguration(
            this IServiceCollection services, 
            IHostEnvironment environment,
            string policyName = DefaultCorsPolicyName)
        {
            if (environment.IsDevelopment())
            {
                // Development configuration - allows any origin but compatible with credentials
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
                // Production configuration - more restrictive
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

        /// <summary>
        /// Enables CORS with the specified policy name
        /// </summary>
        /// <param name="app">The application builder</param>
        /// <param name="policyName">The name of the CORS policy (default: "CorsPolicy")</param>
        /// <returns>The application builder for chaining</returns>
        public static IApplicationBuilder UseCorsConfiguration(
            this IApplicationBuilder app,
            string policyName = DefaultCorsPolicyName)
        {
            return app.UseCors(policyName);
        }
    }
}
