using System.Reflection;

namespace Inventory.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<WarehouseType> WarehouseTypes => Set<WarehouseType>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<MedicineType> MedicineTypes => Set<MedicineType>();
        public DbSet<VaccineType> VaccineTypes => Set<VaccineType>();
        public DbSet<MedicineInteraction> MedicineInteractions => Set<MedicineInteraction>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<SupplierContract> SupplierContracts => Set<SupplierContract>();
        public DbSet<SupplierImportDocument> SupplierImportDocuments => Set<SupplierImportDocument>();
        public DbSet<SupplierImportDocumentDetail> SupplierImportDocumentDetails => Set<SupplierImportDocumentDetail>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
        public DbSet<MedicineBatch> MedicineBatches => Set<MedicineBatch>();
        public DbSet<MedicinePrice> MedicinePrices => Set<MedicinePrice>();
        public DbSet<InventoryDetail> InventoryDetails => Set<InventoryDetail>();
        public DbSet<InventoryHistory> InventoryHistories => Set<InventoryHistory>();
        public DbSet<InventoryLimitStock> InventoryLimitStocks => Set<InventoryLimitStock>();
        public DbSet<MedicineBatchReturn> MedicineBatchReturns => Set<MedicineBatchReturn>();
        public DbSet<MedicineBatchReturnDetail> MedicineBatchReturnDetails => Set<MedicineBatchReturnDetail>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
