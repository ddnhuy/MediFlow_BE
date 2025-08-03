namespace Inventory.Domain.Models
{
    public class SupplierContract
    {
        public Guid Id { get; set; }
        public string? FileName { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
