using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.DAO.Models.Central;

/// <summary>
/// Modelo para almacenar parámetros generales de la aplicación
/// </summary>
public class GeneralParameter
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Key { get; set; } = "";

    [Required]
    [MaxLength(255)]
    public string Value { get; set; } = "";

    [MaxLength(500)]
    public string Description { get; set; } = "";
}
