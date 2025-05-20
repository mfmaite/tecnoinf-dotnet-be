using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DataServices.Services;

public interface ILoginService
{
   UserDTO Login(string user, string password);
}