using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ServiPuntosUy.DAO.Models.Central;

namespace ServiPuntosUy.DAO.Models.Central;

public class PromotionBranch
{
    public int PromotionId { get; set; }
    public Promotion Promotion { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
}