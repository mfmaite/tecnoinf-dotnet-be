using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class ServiceAvailability
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    [Required]
    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    [ForeignKey("Service")]
    [Required]
    public int ServiceId { get; set; }
    public Service Service { get; set; }

    [ForeignKey("Tenant")]
    [Required]
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }

    [Required]
    public TimeOnly StartTime { get; set; } = new TimeOnly(8, 0);

    [Required]
    public TimeOnly EndTime { get; set; } = new TimeOnly(22, 0);
}
