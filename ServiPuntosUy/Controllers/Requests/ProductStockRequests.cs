namespace ServiPuntosUy.Requests;

public class ManageProductStockRequest
{
    public int productId { get; set; }
    public int tenantId { get; set; }
    public int branchId { get; set; }
    public int stock { get; set; }
}
