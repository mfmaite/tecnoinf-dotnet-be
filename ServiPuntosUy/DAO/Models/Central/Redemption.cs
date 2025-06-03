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
    public Branch Branch { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public DateTime RedemptionDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ExpiryDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string QRCode { get; set; } = "";
}
