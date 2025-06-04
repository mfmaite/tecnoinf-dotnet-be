using System.ComponentModel.DataAnnotations;

namespace ServiPuntosUy.Requests
{
    public class UpdateTenantRequest
    {
        [Required(ErrorMessage = "El nombre del tenant es requerido")]
        public string Name { get; set; }
    }
}
