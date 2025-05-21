namespace ServiPuntosUy.DAO.Models.Central;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

  public class CentralUser : BaseUser
  {
    // [ForeignKey("Role")]
    [Column("RoleId", Order = 4)]
    public int? roleId { get; set; }
  }
