namespace Inventory.Infrastructure.Data.Configurations.Warehouses
{
    public class WarehouseTypeConfiguration : IEntityTypeConfiguration<WarehouseType>
    {
        public void Configure(EntityTypeBuilder<WarehouseType> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
