using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.DAO.Models.Central;

public class Tenant
{
    public int Id { get; set; }

    [Required]
    public string TenantId { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
