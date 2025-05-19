using System.Text.Json.Serialization;

namespace ServiPuntosUy.DTO;

public class TenantDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TenantId { get; set; }
}
