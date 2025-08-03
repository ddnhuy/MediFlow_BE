namespace Inventory.Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Warehouse> Warehouses { get; }
        DbSet<WarehouseType> WarehouseTypes { get; }
        DbSet<Medicine> Medicines { get; }
        DbSet<MedicineType> MedicineTypes { get; }
        DbSet<VaccineType> VaccineTypes { get; }
        DbSet<MedicineInteraction> MedicineInteractions { get; }
        DbSet<Supplier> Suppliers { get; }
        DbSet<SupplierContract> SupplierContracts { get; }
        DbSet<SupplierImportDocument> SupplierImportDocuments { get; }
        DbSet<SupplierImportDocumentDetail> SupplierImportDocumentDetails { get; }
        DbSet<Country> Countries { get; }
        DbSet<Manufacturer> Manufacturers { get; }
        DbSet<MedicineBatch> MedicineBatches { get; }
        DbSet<MedicinePrice> MedicinePrices { get; }    
        DbSet<InventoryDetail> InventoryDetails { get; }
        DbSet<InventoryHistory> InventoryHistories { get; }
        DbSet<Inventory.Domain.Models.InventoryLimitStock> InventoryLimitStocks { get; }
        DbSet<MedicineBatchReturn> MedicineBatchReturns { get; }
        DbSet<MedicineBatchReturnDetail> MedicineBatchReturnDetails { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DatabaseFacade Database { get; }
    }
}
