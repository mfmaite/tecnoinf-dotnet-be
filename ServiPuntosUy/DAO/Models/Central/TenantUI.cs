using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class TenantUI
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public string LogoUrl { get; set; } = "";

    [Required]
    public string PrimaryColor { get; set; } = "";

    [Required]
    public string SecondaryColor { get; set; } = "";
}
