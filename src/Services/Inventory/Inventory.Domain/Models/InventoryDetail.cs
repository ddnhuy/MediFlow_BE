namespace Inventory.Domain.Models
{
    public class InventoryDetail : Entity
    {
        public int MedicineBatchId { get; set; }
        public MedicineBatch? MedicineBatch { get; set; }
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public decimal Quantity { get; set; }
        public decimal CostPrice { get; set; }  
    }
}
