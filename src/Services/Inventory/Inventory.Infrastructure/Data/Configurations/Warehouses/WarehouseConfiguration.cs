namespace Inventory.Infrastructure.Data.Configurations.Warehouses
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.WarehouseType)
                .WithMany()
                .HasForeignKey(x => x.WarehouseTypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
