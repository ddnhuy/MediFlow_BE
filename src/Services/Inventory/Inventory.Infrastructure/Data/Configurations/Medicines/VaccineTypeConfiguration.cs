namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class VaccineTypeConfiguration : IEntityTypeConfiguration<VaccineType>
    {
        public void Configure(EntityTypeBuilder<VaccineType> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
