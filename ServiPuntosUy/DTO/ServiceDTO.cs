namespace ServiPuntosUy.DTO
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public bool AgeRestricted { get; set; }
        
        // Información de disponibilidad del servicio
        public ServiceAvailabilityDTO[] Availabilities { get; set; } = Array.Empty<ServiceAvailabilityDTO>();
    }
}
