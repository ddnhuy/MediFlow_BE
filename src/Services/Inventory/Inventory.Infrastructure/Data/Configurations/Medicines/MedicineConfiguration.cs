namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.MedicineType)
                .WithMany()
                .HasForeignKey(x => x.MedicineTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.VaccineType)
                    .WithMany()
                    .HasForeignKey(x => x.VaccineTypeId)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
