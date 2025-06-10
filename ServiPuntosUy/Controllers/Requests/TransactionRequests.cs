namespace ServiPuntosUy.Requests;

public class CreateTransactionRequest
{
    public int BranchId { get; set; }
    public ProductQuantity[] Products { get; set; }
}

public class ProductQuantity
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

