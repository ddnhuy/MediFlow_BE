namespace Inventory.Domain.Models
{
    public sealed class Warehouse : Entity
    {
        public string? WarehouseCode { get; set; } = string.Empty;
        public string? WarehouseName { get; set; } = string.Empty;
    }
}
