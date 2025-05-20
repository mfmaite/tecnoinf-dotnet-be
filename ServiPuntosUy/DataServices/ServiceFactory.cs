using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceFactory
{
    private IServiceCollection _serviceCollection;
    private readonly IConfiguration _configuration;

    public ServiceFactory(IConfiguration configuration)
    {
        _configuration = configuration;
        _serviceCollection = new ServiceCollection();
    }

    public TService GetService<TService>()
    {
        // Construir el ServiceProvider y obtener el servicio solicitado
        return _serviceCollection.BuildServiceProvider().GetRequiredService<TService>();
    }
}