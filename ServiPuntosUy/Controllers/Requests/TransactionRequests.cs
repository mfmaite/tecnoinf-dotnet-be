namespace ServiPuntosUy.Requests;

public class CreateTransactionRequest
{
    public int BranchId { get; set; }
    public int[] ProductIds { get; set; }
}
