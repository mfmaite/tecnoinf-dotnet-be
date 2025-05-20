using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para poder traer relaciones en las entidades.
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Controllers;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;
using ServiPuntosUy.DataServices.CommonLogic;

namespace ServiPuntosUY.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : BaseController
{
    private readonly ILoginService _loginService;
    private readonly IAuthLogic _authLogic;
    
    public LoginController(ILoginService loginService, IAuthLogic authLogic)
    {
        _authLogic = authLogic;
        _loginService = loginService;
    }

    // Endpoint para el login
    [HttpPost("")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
      var user = _loginService.Login(loginRequest.Email, loginRequest.Password);

      if (user == null)
          return Unauthorized("Usuario o contraseña incorrectos");
      
      // Request.Headers.TryGetValue("currentTenant", out var currentTenant); // como todavia no esta logueado, no tiene jwt. 
      var currentTenant = "central"; // se obtiene el tenant del header.

      // Generar el token JWT
      var tokenData     = _authLogic.GenerateToken(user, currentTenant);
      var returnToken   = tokenData.Item1;
      var returnExpires = tokenData.Item2;


      var successResponse = new ApiResponse {
        Error   = false,
        Data    = new {returnToken, returnExpires, loginRequest.Email},
        Message = "Login exitoso"
      };

      return Ok(successResponse);
    }
}