using System.Linq.Expressions;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;


namespace ServiPuntosUy.DataServices.Services.Central;

public class LoginService : ILoginService
{
    private readonly IGenericRepository<Login> _loginRepository; // <login> hace referencia a el DAO de Login

    public LoginService(IGenericRepository<Login> loginRepository)
    {
        _loginRepository = loginRepository;
    }


    public UserDTO Login(string user, string password) {

        return new UserDTO
        {
            Email = user,
            Password = password
        };
        // var condicion = new List<Expression<Func<Tenant, bool>>>
        // {
        //     tenant => tenant.User == user && tenant.Password == password
        // };
    
        //     var tenant = _tenantRepository.GetQueryable().FirstOrDefault(t => t.User == user && t.Password == password);
        //     return tenant != null ? new UserDTO { User = tenant.User, Password = tenant.Password } : null;
    }
}