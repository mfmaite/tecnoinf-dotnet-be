using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiPuntosUy.Enums;

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

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public decimal Amount { get; set; } = 0m;

    [Required]
    public int PointsEarned { get; set; } = 0;

    [Required]
    public TransactionType Type { get; set; } = TransactionType.Purchase;

    [Required]
    public int PointsSpent { get; set; } = 0;
}
