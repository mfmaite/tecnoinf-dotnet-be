namespace ServiPuntosUy.DTO;

public class PromotionExtendedDTO
{
    public int PromotionId { get; set; }
    public int TenantId { get; set; }
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<int> Branches { get; set; } = new List<int>();
    public List<int> Products { get; set; } = new List<int>();
    public int Price { get; set; } = 0;
}