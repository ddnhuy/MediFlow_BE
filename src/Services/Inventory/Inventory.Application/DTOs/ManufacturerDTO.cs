namespace Inventory.Application.DTOs
{
    public class ManufacturerDTO
    {
        public int Id { get; set; }
        public string ManufacturerName { get; set; } = default!;
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}
