namespace ServiPuntosUy.DTO;

/// <summary>
/// DTO para parámetros generales de la aplicación
/// </summary>
public class GeneralParameterDTO
{
    public int Id { get; set; }
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
    public string Description { get; set; } = "";
}
