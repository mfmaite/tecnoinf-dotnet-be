namespace ServiPuntosUy.DTO;

public class ProductDTO
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public TenantDTO Tenant { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public decimal Price { get; set; } = 0m;
    public bool AgeRestricted { get; set; } = false;
}
