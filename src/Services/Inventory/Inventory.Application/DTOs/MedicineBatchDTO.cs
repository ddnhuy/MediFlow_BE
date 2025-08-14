using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.DTOs
{
    public class MedicineBatchDTO
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string? MedicineName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = default!;
        public decimal Quantity { get; set; } = 0;
        public DateOnly ImportDate { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public decimal ImportPrice { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; } = string.Empty;
        public int? ManufacturerId { get; set; }
        public string? ManufacturerName { get; set; } = string.Empty;
        public MedicineBatchStatus Status { get; set; } = MedicineBatchStatus.IsActive;
    }
}
