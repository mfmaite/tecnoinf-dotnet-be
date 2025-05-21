namespace ServiPuntosUy.DTO;

public class EstacionDTO
{
    public int Id { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public int TenantId { get; set; }
    public TenantDTO Tenant { get; set; }
}
