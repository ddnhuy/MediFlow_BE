using BuildingBlocks.Strings.Enums;

namespace Inventory.Domain.Models
{
    public class InventoryLimitStock: Entity
    {
        public int MedicineId { get; set; }
        public Medicine? Medicine { get; set; }
        public decimal MinimalStockThreshold { get; set; }
    }
}
