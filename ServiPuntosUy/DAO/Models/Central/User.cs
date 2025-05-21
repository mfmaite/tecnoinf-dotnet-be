using ServiPuntosUy.Enums;

namespace ServiPuntosUy.DAO.Models.Central;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordSalt { get; set; }
    public UserType Role { get; set; }
    public string TenantId { get; set; } // Null para usuarios Central
    public int? BranchId { get; set; } // Solo para usuarios Branch
}
