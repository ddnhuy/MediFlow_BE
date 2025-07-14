namespace Inventory.Domain.Models
{
    public class WarehouseType : Entity
    {
        public string? WarehouseTypeCode { get; set; } = string.Empty;
        public string? WarehouseTypeName { get; set; } = string.Empty;
    }
}
