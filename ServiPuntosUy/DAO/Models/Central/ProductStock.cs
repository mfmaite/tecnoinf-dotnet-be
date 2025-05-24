using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class ProductStock
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    [Required]
    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    [ForeignKey("Product")]
    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Required]
    public int Stock { get; set; } = 0;
}
