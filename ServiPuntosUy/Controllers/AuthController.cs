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
            var userSession = await AuthService.AuthenticateAsync(request.Email, request.Password);

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
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
            var userSession = await AuthService.Signup(request.Email, request.Password, request.Name, request.TenantId);

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

    }
}
