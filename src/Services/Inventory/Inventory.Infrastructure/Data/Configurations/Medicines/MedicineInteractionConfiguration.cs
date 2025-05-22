namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class MedicineInteractionConfiguration : IEntityTypeConfiguration<MedicineInteraction>
    {
        public void Configure(EntityTypeBuilder<MedicineInteraction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Medicine1)
                .WithMany()
                .HasForeignKey(x => x.MedicineId1)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Medicine2)
                .WithMany()
                .HasForeignKey(x => x.MedicineId2)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
