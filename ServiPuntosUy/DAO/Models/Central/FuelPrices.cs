using ServiPuntosUy.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class FuelPrices
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    [Required]
    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    [Required]
    public FuelType FuelType { get; set; } = FuelType.Super;

    [Required]
    public decimal Price { get; set; } = 0m;
}
