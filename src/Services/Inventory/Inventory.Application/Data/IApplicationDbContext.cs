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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
