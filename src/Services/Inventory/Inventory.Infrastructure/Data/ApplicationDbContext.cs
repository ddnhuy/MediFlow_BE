using Inventory.Application.Data;
using Inventory.Domain.Models;
using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
