using ServiPuntosUy.DTO;

namespace ServiPuntosUy.DataServices.CommonLogic
{
    public interface IAuthLogic
    {
        Tuple<string, DateTime> GenerateToken(UserDTO user, string? currentTenant = null);
    }
}