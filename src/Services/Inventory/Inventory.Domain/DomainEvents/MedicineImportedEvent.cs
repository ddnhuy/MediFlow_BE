namespace Inventory.Domain.DomainEvents
{
    public record MedicineImportedEvent(
        int SupplierImportDocumentId,
        int SupplierImportDocumentDetailId,
        int MedicineId,
        int MedicineBatchId,
        int WarehouseId,
        decimal Quantity,
        decimal UnitPrice,
        string BatchNumber,
        DateOnly ExpiryDate) : IDomainEvent;
}
