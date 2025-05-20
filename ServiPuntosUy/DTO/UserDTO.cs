using System.Text.Json.Serialization;

namespace ServiPuntosUy.DTO;

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public string? Password { get; set; }
    public string? Email { get; set; }

}
