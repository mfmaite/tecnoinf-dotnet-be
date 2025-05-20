using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para poder traer relaciones en las entidades.
using ServiPuntosUy.DataServices.Services;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.DataServices.Services.Central;
using ServiPuntosUy.DTO;
using ServiPuntosUy.Controllers;
using ServiPuntosUy.Controllers.Base;
using ServiPuntosUY.Controllers.Response;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;



namespace ServiPuntosUY.Controllers;

[ApiController]
[Route("api/[controller]")]
// public class ProductosController: ControllerBase
public class LoginController : BaseController
{
    //=============================================== 
    private readonly ILoginService _loginService;
    private readonly IConfiguration _config;
    
    public LoginController(ILoginService loginService, IConfiguration config)
    {
        _loginService = loginService;
        _config = config;

    }

    // Endpoint para el login
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        var user = _loginService.Login(loginRequest.Email, loginRequest.Password);

        if (user == null)
            return Unauthorized("Usuario o contraseña incorrectos");
        
        // Generar el token JWT
        // ======================================================
                // Request.Headers.TryGetValue("currentTenant", out var currentTenant); // como todavia no esta logueado, no tiene jwt. 
        var currentTenant = "central"; // se obtiene el tenant del header.
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, loginRequest.Email),
            // new Claim(ClaimTypes.Name, user.Name),
            // new Claim(ClaimTypes.Role, user.Role?.Id.ToString() ?? "undefined"),
            new Claim("currentTenant", currentTenant ?? string.Empty),
        };
      var expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:MinutesTokenLifeTime"]));

      var token = new JwtSecurityToken(
        _config["Jwt:Issuer"],
        _config["Jwt:Audience"],
        claims,
        expires: expires,
        signingCredentials: credentials);

      var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

      var tokenData = new Tuple<string, DateTime>(stringToken, expires);
      var returnToken = tokenData.Item1;
      var returnExpires = tokenData.Item2;


      var successResponse = new ApiResponse {
        Error = false,
        Data = new {returnToken, returnExpires, loginRequest.Email},
        Message = "Login exitoso"
      };

        return Ok(successResponse);



        // =====================================================


        

        // return Ok(user);
    }

    


    //=============================================== 

    // private readonly ApplicationDbContext _context;

    // public ProductosController(ApplicationDbContext context)
    // {
    //     _context = context;
    // }


    // [HttpGet]
    // public IActionResult GetProductos()
    // {
    //     var productos = _context.Productos.Include(p => p.Categoria).ToList(); // LINQ para obtener productos con categoría
    //     return Ok(productos);
    // }

    // [HttpGet("{productoId}")]
    // public ActionResult<Producto> GetProducto(int productoId) 
    // {
    //     var producto = _context.Productos.Include(p => p.Categoria).FirstOrDefault(x => x.Id == productoId);

    //     if (producto == null)
    //         return NotFound("No se encontró el producto");

    //     return Ok(producto);
    // }

    // [HttpPost]
    // public ActionResult<Producto> PostProducto(ProductoInsert productoInsert)
    // {
    //     var categoria_reference = _context.Categorias.FirstOrDefault(x => x.Id == productoInsert.IdCategoria);

    //     if (categoria_reference == null)
    //         return NotFound("No se encontró la categoría");

    //     var productoNuevo = new Producto() {
    //         Nombre = productoInsert.Nombre,
    //         Descripcion = productoInsert.Descripcion,
    //         Precio = productoInsert.Precio,
    //         Stock = productoInsert.Stock,
    //         IdCategoria = productoInsert.IdCategoria,
    //         Categoria = categoria_reference,
    //     };

    //     _context.Productos.Add(productoNuevo);
    //     _context.SaveChanges();

    //     return CreatedAtAction(nameof(GetProducto),
    //         new { productoId = productoNuevo.Id },
    //         productoNuevo
    //     );
    // }

    // [HttpPut("{productoId}")]
    // public ActionResult<Producto> PutProducto(int productoId, ProductoInsert productoInsert)
    // {
    //     var producto = _context.Productos.Include(p => p.Categoria).FirstOrDefault(x => x.Id == productoId); //prductID se obtiene de la URL
    //     var categoria_reference = _context.Categorias.FirstOrDefault(x => x.Id == productoInsert.IdCategoria);

    //     if (producto == null)
    //         return NotFound("No se encontró el producto");

    //     if (categoria_reference == null)
    //         return NotFound("No se encontró la categoría");

    //     producto.Nombre = productoInsert.Nombre;
    //     producto.Descripcion = productoInsert.Descripcion;
    //     producto.Precio = productoInsert.Precio;
    //     producto.Stock = productoInsert.Stock;
    //     producto.IdCategoria = productoInsert.IdCategoria;
    //     producto.Categoria = categoria_reference;

    //     _context.Productos.Update(producto);
    //     _context.SaveChanges();

    //     return Ok(producto);
    // }

    // [HttpDelete("{productoId}")]
    // public ActionResult DeleteProducto(int productoId)
    // {
    //     var producto = _context.Productos.FirstOrDefault(x => x.Id == productoId);

    //     if (producto == null)
    //         return NotFound("No se encontró el producto");

    //     _context.Productos.Remove(producto);
    //     _context.SaveChanges();

    //     return Ok(_context.Productos.Include(p => p.Categoria).ToList()); // 204 No Content????
    // }
}