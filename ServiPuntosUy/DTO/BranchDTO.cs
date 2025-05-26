namespace ServiPuntosUy.DTO;

public class BranchDTO
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public TenantDTO Tenant { get; set; }
    public string Address { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Phone { get; set; }
    public TimeOnly OpenTime { get; set; }
    public TimeOnly ClosingTime { get; set; }
}
