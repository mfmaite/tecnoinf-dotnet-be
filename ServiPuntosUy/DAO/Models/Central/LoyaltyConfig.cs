using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class LoyaltyConfig
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public string PointsName { get; set; } = "";

    [Required]
    public int PointsValue { get; set; } = 1;

    [Required]
    public decimal AccumulationRule { get; set; } = 1m; // Cantidad de pesos para acumular un punto

    [Required]
    public int ExpiricyPolicyDays { get; set; } = 180; // La cantidad de días sin login para que se venzan los puntos
}
