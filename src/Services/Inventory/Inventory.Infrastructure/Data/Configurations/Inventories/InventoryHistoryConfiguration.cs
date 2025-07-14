namespace Inventory.Infrastructure.Data.Configurations.Inventories
{
    public class InventoryHistoryConfiguration : IEntityTypeConfiguration<InventoryHistory>
    {
        public void Configure(EntityTypeBuilder<InventoryHistory> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Medicine)
                .WithMany()
                .HasForeignKey(x => x.MedicineId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.MedicineBatch)
                .WithMany()
                .HasForeignKey(x => x.MedicineBatchId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
