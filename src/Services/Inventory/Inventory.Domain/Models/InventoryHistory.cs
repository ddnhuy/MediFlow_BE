namespace Inventory.Domain.Models
{
    public class InventoryHistory : Entity
    {
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public int MedicineBatchId { get; set; }
        public MedicineBatch? MedicineBatch { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Description { get; set; }
    }
}
