namespace Inventory.Application.DTOs
{
    public class WarehouseDTO
    {
        public int Id { get; set; }
        public string? WarehouseCode { get; set; }
        public string? WarehouseName { get; set; }
        public int WarehouseTypeId { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
