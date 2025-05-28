namespace ServiPuntosUy.Requests;

public class AddNewProductRequest
{
    public int tenantId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; } = 0m;
    public bool AgeRestricted { get; set; } = false;
}
