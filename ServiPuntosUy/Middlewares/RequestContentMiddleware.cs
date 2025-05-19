using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;
using ServiPuntosUy.DataServices;


namespace ServiPuntosUy.Middlewares
{
  public class RequestContentMiddleware
  {
    private readonly RequestDelegate _next;

    public RequestContentMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceFactory _serviceFactory)
    {
        // _serviceFactory.CreatePlatformServices();
        await _next(context);
    }
  }
}
