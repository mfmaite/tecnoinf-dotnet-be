using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DAO.Models.Central;

public class PromotionProduct
{
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
}