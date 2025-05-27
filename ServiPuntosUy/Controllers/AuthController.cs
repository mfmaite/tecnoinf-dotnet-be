using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUy.DataServices;
using ServiPuntosUy.DTO;
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
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var userSession = await AuthService.AuthenticateAsync(request.Email, request.Password);

            if (userSession == null || string.IsNullOrEmpty(userSession.token))
                return Unauthorized(new { message = "Email o contraseña incorrectos" });

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
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Token inválido" });

            var userId = int.Parse(userIdClaim.Value);
            var user = await AuthService.GetUserInfoAsync(userId);

            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(user);
        }
    }
}
