namespace ServiPuntosUy.Requests;


public class CreateBranchRequest
{
    public string TenantId { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
}