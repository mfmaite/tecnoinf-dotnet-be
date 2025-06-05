using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests;

public class CreateTenantRequest
{
    [Required(ErrorMessage = "El nombre del tenant es requerido")]
    public string Name { get; set; } = "";
}

public class UpdateTenantRequest
{
    [Required(ErrorMessage = "El nombre del tenant es requerido")]
    public string Name { get; set; }
}
