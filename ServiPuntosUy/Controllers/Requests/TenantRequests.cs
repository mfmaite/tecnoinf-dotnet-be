namespace ServiPuntosUy.Requests;

public class CreateTenantRequest
{
    public string Name { get; set; } = "";
    public string PointsName { get; set; } = "";
    public int PointsValue { get; set; } = 1;
    public decimal AccumulationRule { get; set; } = 1m;
    public int ExpiricyPolicyDays { get; set; } = 180;
}
