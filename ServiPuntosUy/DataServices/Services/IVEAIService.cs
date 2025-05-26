using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.Services
{
    /// <summary>
    /// Interfaz para el servicio de VEAI
    /// </summary>
    public interface IVEAIService
    {
        Task<UserDTO> VerificarIdentidad(int userId, string nroDoc);
    }
}
