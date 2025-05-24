using ServiPuntosUy.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class User
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tenant")]
    public int? TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public string Email { get; set; } = "";

    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";

    [Required]
    public string PasswordSalt { get; set; } = "";

    [Required]
    public UserType Role { get; set; } = UserType.Central;

    [ForeignKey("Branch")]
    public int? BranchId { get; set; } // Es NULL si no es admin de branch
    public Branch Branch { get; set; }

    public bool IsVerified { get; set; } = false;

    public int PointBalance { get; set; } = 0;

    public bool NotificationsEnabled { get; set; } = true;

    public DateTime LastLoginDate { get; set; }
}
