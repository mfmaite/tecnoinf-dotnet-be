namespace ServiPuntosUy.Requests;

public class CreateBranchRequest
{
    public string Address { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Phone { get; set; }
    public string OpenTime { get; set; }
    public string ClosingTime { get; set; }
}

public class UpdateBranchRequest
{
    public string? Address { get; set; }
    public string? Latitud { get; set; }
    public string? Longitud { get; set; }
    public string? Phone { get; set; }
    public string? OpenTime { get; set; }
    public string? ClosingTime { get; set; }
}

public class SetBranchHoursRequest
{
    public string OpenTime { get; set; }
    public string ClosingTime { get; set; }
    public int branchId { get; set; }

}
