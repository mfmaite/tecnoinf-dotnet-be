using System.Text.Json.Serialization;

namespace ServiPuntosUy.DTO;

public class TenantDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    [JsonIgnore]
    public string Password { get; set; } = string.Empty;
}
