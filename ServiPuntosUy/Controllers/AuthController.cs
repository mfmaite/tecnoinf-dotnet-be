using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Requests;
using ServiPuntosUY.Controllers.Response;
using System.Security.Claims;

namespace ServiPuntosUy.Controllers
{
    /// <summary>
    /// Controlador para la autenticación de usuarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        public AuthController(IServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var userSession = await AuthService.AuthenticateAsync(request.Email, request.Password, HttpContext);

            if (userSession == null || string.IsNullOrEmpty(userSession.token))
                return Unauthorized(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Email o contraseña incorrectos"
                });

            return Ok(new ApiResponse<UserSessionDTO>
            {
                Error = false,
                Message = "Inicio de sesión correcto",
                Data = userSession
            });
        }

        /// <summary>
        /// Genera un magic link para el login
        /// </summary>
        /// <param name="request">Email del usuario</param>
        /// <returns>Token del magic link</returns>
        [HttpPost("magic-link")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> GenerateMagicLink([FromBody] MagicLinkRequest request)
        {
            try
            {
                var magicLinkToken = await AuthService.GenerateMagicLinkAsync(request.Email, HttpContext);
                return Ok(new ApiResponse<string>
                {
                    Error = false,
                    Message = "Magic link generado correctamente",
                    Data = magicLinkToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Valida un magic link y genera una sesión
        /// </summary>
        /// <param name="request">Token del magic link</param>
        /// <returns>Token de sesión</returns>
        [HttpPost("validate-magic-link")]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> ValidateMagicLink([FromBody] ValidateMagicLinkRequest request)
        {
            try
            {
                var userSession = await AuthService.ValidateMagicLinkAsync(request.Token);
                return Ok(new ApiResponse<UserSessionDTO>
                {
                    Error = false,
                    Message = "Magic link validado correctamente",
                    Data = userSession
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene información del usuario actual
        /// </summary>
        /// <returns>Información del usuario</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("userId");
            if (userIdClaim == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Token inválido"
                });

            var userId = int.Parse(userIdClaim.Value);
            var user = await AuthService.GetUserInfoAsync(userId);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Error = true,
                    Message = "Usuario no encontrado"
                });

            return Ok(new ApiResponse<UserDTO>
            {
                Error = false,
                Message = "Usuario obtenido correctamente",
                Data = user
            });
        }

        /// <summary>
        /// Registra a un usuario
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        /// <returns>Token JWT</returns>
        [HttpPost("signup")]
        [ProducesResponseType(typeof(ApiResponse<UserSessionDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request)
        {
            try
            {
                // Obtener el tenantId del contexto HTTP (jwt o header)
                var tenantIdStr = HttpContext.Items["CurrentTenant"] as string;
                if (string.IsNullOrEmpty(tenantIdStr) || !int.TryParse(tenantIdStr, out int tenantId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Error = true,
                        Message = "No se pudo determinar el tenant para el registro"
                    });
                }

                var userSession = await AuthService.Signup(request.Email, request.Password, request.Name, tenantId, HttpContext);

                if (userSession == null || string.IsNullOrEmpty(userSession.token))
                    return Unauthorized(new ApiResponse<object>
                    {
                        Error = true,
                        Message = "Error al registrar usuario"
                    });

                return Ok(new ApiResponse<UserSessionDTO>
                {
                    Error = false,
                    Message = "Usuario registrado correctamente",
                    Data = userSession
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Error = true,
                    Message = ex.Message
                });
            }
        }

    }
}
