using System.Linq.Expressions;
using ServiPuntosUy.DTO;
using ServiPuntosUy.DAO.Models.Central;
using ServiPuntosUy.Models.DAO;


namespace ServiPuntosUy.DataServices.Services.Central;

public class LoginService : ILoginService
{
    private readonly IGenericRepository<Login> _loginRepository; // <login> hace referencia a el DAO de Login
    private readonly IGenericRepository<CentralUser> _userDAL;

    public LoginService(IGenericRepository<Login> loginRepository, IGenericRepository<CentralUser> userDAL)
    {
        _loginRepository = loginRepository;
        _userDAL = userDAL;
    }


    public UserDTO Login(string email, string password) {

        var user = _userDAL.Get(new List<Expression<Func<CentralUser, bool>>>
        {
            u => u.Email == email && u.Password == password
        }).Select(usr => new UserDTO
        {
            Email = usr.Email,
            Password = usr.Password
        }).FirstOrDefault();

        return user;
    }
}