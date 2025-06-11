using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Controllers.Requests
{
    public class CreateServiceRequest
    {
        [Required]
        public string Name { get; set; } = "";
        
        public string Description { get; set; } = "";
        
        [Required]
        public decimal Price { get; set; }
        
        public bool AgeRestricted { get; set; } = false;

        [Required]
        public TimeOnly StartTime { get; set; }
        
        [Required]
        public TimeOnly EndTime { get; set; }
    }

    public class UpdateServiceRequest
    {
        [Required]
        public string Name { get; set; } = "";
        
        public string Description { get; set; } = "";
        
        [Required]
        public decimal Price { get; set; }
        
        public bool AgeRestricted { get; set; } = false;

        public TimeOnly? StartTime { get; set; }
        
        public TimeOnly? EndTime { get; set; }
    }


}
