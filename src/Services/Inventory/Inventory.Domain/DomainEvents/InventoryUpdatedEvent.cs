namespace Inventory.Domain.DomainEvents
{
    public record InventoryUpdatedEvent(
        int MedicineId,
        int MedicineBatchId,
        string BatchNumber,
        int WarehouseId,
        decimal Quantity,
        decimal CostPrice,
        decimal UnitPrice) : IDomainEvent;
}
