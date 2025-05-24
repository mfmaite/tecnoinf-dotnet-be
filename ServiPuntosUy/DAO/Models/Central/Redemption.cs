using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class Redemption
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    [Required]
    public int UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("Branch")]
    [Required]
    public int BranchId { get; set; }
    public Tenant Branch { get; set; }

    [Required]
    public DateTime RedemptionDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string QRCode { get; set; } = "";
}
