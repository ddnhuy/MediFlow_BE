namespace Inventory.Infrastructure.Data.Configurations.Manufacturers
{
    public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
    {
        public void Configure(EntityTypeBuilder<Manufacturer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ManufacturerName)
                .IsRequired();

            builder.HasIndex(x => x.ManufacturerName);
        }
    }
}
