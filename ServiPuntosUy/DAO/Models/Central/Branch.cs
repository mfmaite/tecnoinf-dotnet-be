using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Tenant")]
        [Required]
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }

        [Required]
        public string Address { get; set; } = "";

        [Required]
        public string Latitud { get; set; } = "";

        [Required]
        public string Longitud { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        [Required]
        public TimeOnly OpenTime { get; set; } = new TimeOnly(8, 0);

        [Required]
        public TimeOnly ClosingTime { get; set; } = new TimeOnly(22, 0);
    }
}
