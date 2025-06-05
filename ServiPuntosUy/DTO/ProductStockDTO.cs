namespace ServiPuntosUy.DTO;

public class ProductStockDTO
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public BranchDTO Branch { get; set; }
    public int ProductId { get; set; }
    public ProductDTO Product { get; set; }
    public int Stock { get; set; } = 0;
}