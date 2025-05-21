using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntosUy.DAO.Models.Central
{
    public class Branch
    {
        public int Id { get; set; }

        [Required]
        public string Latitud { get; set; }

        [Required]
        public string Longitud { get; set; }

        [ForeignKey("Tenant")]
        public int TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }
}
