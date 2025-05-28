using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
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

        public TenantResolver(IConfiguration configuration, CentralDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }


        public string ResolveTenantId(HttpContext context)
        {
            // 1. Intentar obtener del token JWT
            string tenantId = GetTenantIdFromToken(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            // 2. Intentar obtener del host
            tenantId = GetTenantIdFromHost(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            // 3. Intentar obtener del header custom (X-Tenant-Name) en caso de mobile
            tenantId = GetTenantIdFromCustomHeader(context);
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            // 3. Si no se pudo resolver, devolver null o un valor por defecto
            return null;
        }


        public UserType ResolveUserType(HttpContext context)
        {
            // 1. Intentar obtener del token JWT
            UserType? userType = GetUserTypeFromToken(context);
            if (userType.HasValue)
            {
                return userType.Value;
            }

            // 2. Intentar obtener del host
            userType = GetUserTypeFromHost(context);
            if (userType.HasValue)
            {
                return userType.Value;
            }

            // 3. Si no se pudo resolver, asumir usuario final
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
        private string GetTenantIdByName(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
                return null;

            var tenant = _dbContext.Tenants.FirstOrDefault(t => t.Name == tenantName);
            return tenant?.Id.ToString();
        }

        private string GetTenantIdFromHost(HttpContext context)
        {
            string host = context.Request.Host.Host;

            // Patrones de host para diferentes tipos de usuario
            // admin.servipuntos.uy -> Central (no tiene tenant)
            // {tenant-name}.admin.servipuntos.uy -> Tenant
            // {tenant-name}.branch.admin.servipuntos.uy -> Branch
            // {tenant-name}.app.servipuntos.uy -> EndUser (el tenant se obtiene del subdominio)

            string tenantName = null;

            // Para administrador de tenant: {tenant-name}.admin.servipuntos.uy
            var tenantAdminRegex = new Regex(@"^([^.]+)\.admin\.");
            var tenantAdminMatch = tenantAdminRegex.Match(host);
            if (tenantAdminMatch.Success)
            {
                tenantName = tenantAdminMatch.Groups[1].Value;
                return GetTenantIdByName(tenantName);
            }

            // Para administrador de estación: {tenant-name}.branch.admin.servipuntos.uy
            var branchAdminRegex = new Regex(@"^([^.]+)\.branch\.");
            var branchAdminMatch = branchAdminRegex.Match(host);
            if (branchAdminMatch.Success)
            {
                tenantName = branchAdminMatch.Groups[1].Value;
                return GetTenantIdByName(tenantName);
            }

            // Para usuario final: {tenant-name}.app.servipuntos.uy
            var endUserTenantRegex = new Regex(@"^([^.]+)\.app\.");
            var endUserTenantMatch = endUserTenantRegex.Match(host);
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

            // Patrones de host para diferentes tipos de usuario
            if (host.StartsWith("admin."))
            {
                return UserType.Central;
            }
            else if (host.Contains(".admin."))
            {
                return UserType.Tenant;
            }
            else if (host.Contains(".branch."))
            {
                return UserType.Branch;
            }
            else if (host.Contains(".app."))
            {
                return UserType.EndUser;
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
