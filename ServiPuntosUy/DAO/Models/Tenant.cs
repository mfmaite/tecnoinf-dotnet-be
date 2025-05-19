using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.DAO.Models;

public class Tenant
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string TenantId { get; set; } = string.Empty;
}
