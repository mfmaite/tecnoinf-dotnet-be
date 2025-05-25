namespace ServiPuntosUy.Requests;

public class CreateBranchRequest
{
    public int TenantId { get; set; }
    public string Address { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Phone { get; set; }
    public string OpenTime { get; set; }
    public string ClosingTime { get; set; }
}
