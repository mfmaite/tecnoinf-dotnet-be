namespace ServiPuntosUy.DTO;

public class ProductWithStockDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; } = 0m;
    public bool AgeRestricted { get; set; } = false;
    public int Stock { get; set; } = 0;
}
