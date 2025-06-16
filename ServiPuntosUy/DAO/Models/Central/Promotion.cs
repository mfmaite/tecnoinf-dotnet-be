using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class Promotion
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    public string Description { get; set; } = "";

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    public int Price { get; set; } = 0;

    // Propiedad de navegación inversa
    public ICollection<PromotionBranch> PromotionBranch { get; set; }
    public ICollection<PromotionProduct> PromotionProduct { get; set; }

}
