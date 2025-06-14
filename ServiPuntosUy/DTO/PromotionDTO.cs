namespace ServiPuntosUy.DTO;

public class PromotionDTO
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public TenantDTO Tenant { get; set; }
    public int? BranchId { get; set; }
    public BranchDTO Branch { get; set; }
    public string Description { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}