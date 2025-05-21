namespace ServiPuntosUy.DTO;

public class ProductDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Price { get; set; }
    public int EstacionId { get; set; }
    public EstacionDTO Estacion { get; set; }
}
