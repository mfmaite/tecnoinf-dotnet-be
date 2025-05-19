using System;
using ServiPuntosUy.DataServices; 
namespace ServiPuntosUy.DataServices
{
    public interface IServiceFactory
    {
        T GetService<T>() where T : class;
    }
}