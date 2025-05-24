using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class Service
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    [Required]
    public decimal Price { get; set; } = 0m;

    [Required]
    public bool AgeRestricted { get; set; } = false;
}
