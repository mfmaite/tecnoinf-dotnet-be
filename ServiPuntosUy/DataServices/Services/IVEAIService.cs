namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de VEAI
    /// </summary>
    public interface IVEAIService
    {
        Task<string> ObtenerNombrePersona(string nroDoc, string tipoDoc, string nroSerie, string organismo, string clave);
    }
}
