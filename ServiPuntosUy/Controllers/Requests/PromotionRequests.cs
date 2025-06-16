namespace ServiPuntosUy.Requests;

public class AddNewPromotionRequest
{
    public int tenantId { get; set; }
    public int branchId { get; set; }
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public List<int> Branch { get; set; } = new List<int>();
    public List<int> Product { get; set; } = new List<int>();
    public int Price { get; set; } = 0;
}

public class UpdatePromotionRequest 
{
    public int PromotionId { get; set; }
    public int tenantId { get; set; }
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);
    public List<int> Branch { get; set; } = new List<int>();
    public List<int> Product { get; set; } = new List<int>();
    public int Price { get; set; } = 0;
}