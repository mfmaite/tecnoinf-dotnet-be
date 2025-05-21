using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DataServices.Services;

public interface IEstacionService
{
    EstacionDTO GetEstacionDTO(Estacion estacion);
    EstacionDTO GetEstacionById(int id);
    EstacionDTO CreateEstacion(string latitud, string longitud, string tenantId);
}
