using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central;

public class TransactionItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Transaction")]
    [Required]
    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; }

    [ForeignKey("Product")]
    [Required]
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Required]
    public int Quantity { get; set; } = 1;

    [Required]
    public decimal UnitPrice { get; set; }
    
    // Precio original antes del descuento (para referencia)
    public decimal OriginalPrice { get; set; }
    
    // Referencia a la promoción aplicada (si existe)
    [ForeignKey("Promotion")]
    public int? PromotionId { get; set; }
    public Promotion Promotion { get; set; }
}
