using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class SatisfactionSurvey
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    public int? BranchId { get; set; }
    public Branch Branch { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public int Rating { get; set; } = 1;

    [Required]
    public string Comments { get; set; } = "";

    [Required]
    public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
}
