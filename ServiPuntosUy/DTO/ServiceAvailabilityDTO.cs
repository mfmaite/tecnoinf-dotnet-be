namespace ServiPuntosUy.DTO
{
    public class ServiceAvailabilityDTO
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ServiceId { get; set; }
        public int TenantId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ServiceName { get; set; } = ""; // Para mostrar información adicional
    }
}
