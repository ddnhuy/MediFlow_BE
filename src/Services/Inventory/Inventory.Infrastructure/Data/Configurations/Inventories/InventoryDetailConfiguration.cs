namespace Inventory.Infrastructure.Data.Configurations.Inventories
{
    public class InventoryDetailConfiguration : IEntityTypeConfiguration<InventoryDetail>
    {
        public void Configure(EntityTypeBuilder<InventoryDetail> builder)
        {
            builder.HasKey(x => x.Id);

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
