using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    [Required]
    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public decimal Amount { get; set; } = 0m;

    [Required]
    public int PointsEarned { get; set; } = 0;
}
