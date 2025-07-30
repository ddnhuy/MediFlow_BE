using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.DTOs
{
    public class InventoryLimitStockDTO
    {
        private const string CRITICAL_LOW_STATUS = "Critical Low";
        private const string LOW_STATUS = "Low";
        private const string NORMAL_STATUS = "Normal";

        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string? MedicineCode { get; set; }
        public string? MedicineName { get; set; }
        public string? Unit { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimalStockThreshold { get; set; }
        public decimal Difference { get; set; }
        public InventoryLimitStockStatus InventoryLimitStockStatus { get; set; }

        public string StatusDescription => InventoryLimitStockStatus switch
        {
            InventoryLimitStockStatus.CriticalLow => CRITICAL_LOW_STATUS,
            InventoryLimitStockStatus.Low => LOW_STATUS,
            InventoryLimitStockStatus.Normal => NORMAL_STATUS,
            _ => NORMAL_STATUS
        };

        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}
