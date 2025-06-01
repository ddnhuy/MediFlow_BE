namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class MedicineBatchConfiguration : IEntityTypeConfiguration<MedicineBatch>
    {
        public void Configure(EntityTypeBuilder<MedicineBatch> builder)
        {
            builder.HasKey(mb => mb.Id);

            builder.Property(mb => mb.BatchNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(mb => mb.ImportPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(mb => mb.CostPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure relationships
            builder.HasOne(mb => mb.Supplier)
                .WithMany()
                .HasForeignKey(mb => mb.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(mb => mb.Manufacturer)
                .WithMany()
                .HasForeignKey(mb => mb.ManufacturerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
