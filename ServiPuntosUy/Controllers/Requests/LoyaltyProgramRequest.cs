using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests;

public class CreateLoyaltyProgramRequest
{
  [Required(ErrorMessage = "El nombre de los puntos es requerido")]
  public string PointsName { get; set; }

  [Required(ErrorMessage = "El valor de los puntos es requerido")]
  public int PointsValue { get; set; }

  [Required(ErrorMessage = "La regla de acumulación de puntos es requerida")]
  public decimal AccumulationRule { get; set; }

  [Required(ErrorMessage = "El día de expiración de los puntos es requerido")]
  public int ExpiricyPolicyDays { get; set; }
}
