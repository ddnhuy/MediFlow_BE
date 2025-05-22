namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class MedicineTypeConfiguration : IEntityTypeConfiguration<MedicineType>
    {
        public void Configure(EntityTypeBuilder<MedicineType> builder)
        {
            builder.HasKey(x => x.Id);          
        }
    }
}
