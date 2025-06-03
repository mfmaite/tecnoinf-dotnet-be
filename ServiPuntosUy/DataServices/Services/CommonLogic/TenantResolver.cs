using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using ServiPuntosUy.DAO.Data.Central;
using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DataServices.Services.CommonLogic
{
    /// <summary>
    /// Implementación para resolver el tenant y tipo de usuario a partir del contexto HTTP
    /// </summary>
    public class TenantResolver : ITenantResolver
    {
        private readonly IConfiguration _configuration;
        private readonly CentralDbContext _dbContext;
        private readonly IHostEnvironment _environment;

        public TenantResolver(IConfiguration configuration, CentralDbContext dbContext, IHostEnvironment environment)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _environment = environment;
        }


        public string ResolveTenantId(HttpContext context)
        {
            // 1. Si el usuario está autenticado, obtener el tenant ID del token JWT
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                string tenantId = GetTenantIdFromToken(context);
                if (!string.IsNullOrEmpty(tenantId))
                {
                    return tenantId;
                }
            }

            // 2. Intentar obtener del host (GetTenantIdFromHost ya maneja el caso de admin.*)
            string tenantIdFromHost = GetTenantIdFromHost(context);
            if (!string.IsNullOrEmpty(tenantIdFromHost))
            {
                return tenantIdFromHost;
            }

            // 3. Intentar obtener del header custom (X-Tenant-Name)
            string tenantIdFromHeader = GetTenantIdFromCustomHeader(context);
            if (!string.IsNullOrEmpty(tenantIdFromHeader))
            {
                return tenantIdFromHeader;
            }

            // 4. Si no se pudo resolver, devolver null
            return null;
        }


        public UserType ResolveUserType(HttpContext context)
        {
            // 1. Si el usuario está autenticado, obtener el user type del token JWT
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                UserType? userType = GetUserTypeFromToken(context);
                if (userType.HasValue)
                {
                    return userType.Value;
                }
            }

            // 2. Intentar obtener del host (GetUserTypeFromHost ya maneja el caso de admin.*)
            UserType? userTypeFromHost = GetUserTypeFromHost(context);
            if (userTypeFromHost.HasValue)
            {
                return userTypeFromHost.Value;
            }

            // 3. Intentar obtener de un custom header (X-User-Type) -> (solo para dev mode)
            UserType? userTypeFromCustomHeader = GetUserTypeFromCustomHeader(context);
            if (userTypeFromCustomHeader.HasValue)
            {
                return userTypeFromCustomHeader.Value;
            }

            // 4. Si no se pudo resolver, asumir usuario final
            return UserType.EndUser;
        }

        /// <summary>
        /// Resuelve el ID de la estación a partir del contexto HTTP (solo para administradores de estación)
        /// </summary>
        /// <param name="context">Contexto HTTP</param>
        /// <returns>ID de la estación, o null si no aplica</returns>
        public int? ResolveBranchId(HttpContext context)
        {
            // 1. Intentar obtener del token JWT
            int? branchId = GetBranchIdFromToken(context);
            if (branchId.HasValue)
            {
                return branchId.Value;
            }

            // 2. Intentar obtener de la URL
            branchId = GetBranchIdFromUrl(context);
            if (branchId.HasValue)
            {
                return branchId.Value;
            }

            // 3. Si no se pudo resolver, devolver null
            return null;
        }


        private string GetTenantIdFromToken(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var tenantId = jwtToken.Claims.FirstOrDefault(x => x.Type == "tenantId")?.Value;

                return tenantId;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Busca el ID del tenant a partir de su nombre
        /// </summary>
        /// <param name="tenantName">Nombre del tenant</param>
        /// <returns>ID del tenant como string, o null si no se encuentra</returns>
        private string? GetTenantIdByName(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
                return null;

            var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Name == tenantName);
            return tenant?.Id.ToString();
        }

        private string GetTenantIdFromHost(HttpContext context)
        {
            string host = context.Request.Host.Host;

            // Para administradores: admin.servipuntos.uy
            // No intentamos resolver el tenant desde la URL
            if (host.StartsWith("admin."))
            {
                return null; // Devolvemos null para que el sistema use el JWT
            }

            // Para usuario final: {tenant-name}.app.servipuntos.uy
            var endUserTenantRegex = new Regex(@"^([^.]+)\.app\.");
            var endUserTenantMatch = endUserTenantRegex.Match(host);
            string tenantName;
            if (endUserTenantMatch.Success)
            {
                tenantName = endUserTenantMatch.Groups[1].Value;
                return GetTenantIdByName(tenantName);
            }

            // Para desarrollo local, se puede usar un query parameter
            if (context.Request.Query.TryGetValue("tenant", out var tenantParam))
            {
                tenantName = tenantParam.ToString();
                return GetTenantIdByName(tenantName);
            }

            return null;
        }

        private string GetTenantIdFromCustomHeader(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Tenant-Name", out var tenantNameHeader))
            {
                // Obtener el nombre del tenant del header
                string tenantName = tenantNameHeader.ToString();
                if (!string.IsNullOrWhiteSpace(tenantName))
                {
                    // Buscar el ID a partir del nombre
                    return GetTenantIdByName(tenantName);
                }
            }

            return null;
        }

        private UserType? GetUserTypeFromCustomHeader(HttpContext context)
        {
            // Solo procesar el header en entorno de desarrollo
            if (!_environment.IsDevelopment())
            {
                return null;
            }

            if (context.Request.Headers.TryGetValue("X-User-Type", out var userTypeHeader))
            {
                // Obtener el nombre del tenant del header
                string userType = userTypeHeader.ToString();
                if (!string.IsNullOrWhiteSpace(userType))
                {
                    // Buscar el ID a partir del nombre
                    return (UserType)Enum.Parse(typeof(UserType), userType);
                }
            }

            return null;
        }


        private UserType? GetUserTypeFromToken(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var userTypeClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userType")?.Value;

                if (userTypeClaim != null && Enum.TryParse<UserType>(userTypeClaim, out var userType))
                {
                    return userType;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private UserType? GetUserTypeFromHost(HttpContext context)
        {
            string host = context.Request.Host.Host;

            // Para usuarios finales: {tenant-name}.app.servipuntos.uy
            if (host.Contains(".app."))
            {
                return UserType.EndUser;
            }

            // Para administradores: admin.servipuntos.uy
            // No determinamos el tipo específico de administrador desde la URL
            if (host.StartsWith("admin."))
            {
                // Devolvemos null para que el sistema use el JWT
                return null;
            }

            // Para desarrollo local, se puede usar un query parameter
            if (context.Request.Query.TryGetValue("userType", out var userTypeParam) &&
                Enum.TryParse<UserType>(userTypeParam.ToString(), out var userType))
            {
                return userType;
            }

            return null;
        }

        private int? GetBranchIdFromToken(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(authHeader.Split(' ')[1].Trim());
                var branchIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "branchId")?.Value;

                if (branchIdClaim != null && int.TryParse(branchIdClaim, out int branchId))
                {
                    return branchId;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private int? GetBranchIdFromUrl(HttpContext context)
        {
            // Buscar en la ruta
            var routeData = context.GetRouteData();
            if (routeData?.Values != null && routeData.Values.TryGetValue("branchId", out var branchIdRoute) &&
                int.TryParse(branchIdRoute.ToString(), out int branchIdFromRoute))
            {
                return branchIdFromRoute;
            }

            // Buscar en los query parameters
            if (context.Request.Query.TryGetValue("branchId", out var branchIdParam) &&
                int.TryParse(branchIdParam.ToString(), out int branchIdFromQuery))
            {
                return branchIdFromQuery;
            }

            return null;
        }

    }
}
