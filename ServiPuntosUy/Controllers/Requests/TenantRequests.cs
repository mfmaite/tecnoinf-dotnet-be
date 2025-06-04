using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests;

public class CreateTenantRequest
{
    [Required(ErrorMessage = "El nombre del tenant es requerido")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "El nombre de los puntos es requerido")]
    public string PointsName { get; set; } = "";

    [Required(ErrorMessage = "El valor de los puntos es requerido")]
    public int PointsValue { get; set; } = 1;

    [Required(ErrorMessage = "La regla de acumulación de puntos es requerida")]
    public decimal AccumulationRule { get; set; } = 1m;

    [Required(ErrorMessage = "El día de expiración de los puntos es requerido")]
    public int ExpiricyPolicyDays { get; set; } = 180;
}
