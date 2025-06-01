namespace Inventory.Application.Medicines.Commands.ImportMedicineFromSupplier
{
    public record ImportMedicineFromSupplierCommand : ICommand<ImportMedicineFromSupplierResult>
    {
        public string? DocumentCode { get; init; }
        public string? DocumentNumber { get; init; }
        public int WarehouseId { get; init; }
        public DateOnly ImportDate { get; init; }
        public int SupplierId { get; init; }
        public string? Note { get; init; }
        public int ReceivedById { get; init; }
        public string? SupportingDocument { get; init; }
        public DateOnly EndDate { get; init; }
        public List<ImportMedicineDetailDto> Details { get; init; } = [];
    }

    public record ImportMedicineDetailDto
    {
        public int MedicineId { get; init; }
        public string BatchNumber { get; init; } = string.Empty;
        public string? SGK_CPNK { get; init; }
        public string? Note { get; init; }
        public decimal Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public DateOnly ExpiryDate { get; init; }
        public int ManufacturerId { get; init; }
        public int CountryId { get; init; }
        public bool IsFree { get; init; }
    }

    public record ImportMedicineFromSupplierResult(int SupplierImportDocumentId);
}
