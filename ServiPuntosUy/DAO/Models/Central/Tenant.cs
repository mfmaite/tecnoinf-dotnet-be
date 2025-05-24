using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.DAO.Models.Central;

public class Tenant
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";
}
