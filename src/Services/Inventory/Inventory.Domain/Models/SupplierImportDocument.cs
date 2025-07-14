namespace Inventory.Domain.Models
{
    public class SupplierImportDocument : Entity
    {
        public string? DocumentCode { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public Warehouse? Warehouse { get; set; }
        public DateOnly ImportDate { get; set; }
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; } = default!;
        public string? Note { get; set; } = string.Empty;
        public int ReceivedById { get; set; }
        public string? SupportingDocument { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
