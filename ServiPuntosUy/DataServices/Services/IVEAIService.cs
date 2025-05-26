namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de VEAI
    /// </summary>
    public interface IVEAIService
    {
        Task<string> VerificarIdentidad(int userId, string nroDoc);
    }
}
