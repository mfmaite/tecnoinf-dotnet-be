using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public int Price { get; set; }

    [ForeignKey("Estacion")]
    public int EstacionId { get; set; }
    public Estacion Estacion { get; set; }
}
