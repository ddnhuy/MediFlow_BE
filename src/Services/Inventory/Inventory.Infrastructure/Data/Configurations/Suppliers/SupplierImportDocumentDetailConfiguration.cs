namespace Inventory.Infrastructure.Data.Configurations.Suppliers
{
    public class SupplierImportDocumentDetailConfiguration : IEntityTypeConfiguration<SupplierImportDocumentDetail>
    {
        public void Configure(EntityTypeBuilder<SupplierImportDocumentDetail> builder)
        {
            builder.HasKey(sidd => sidd.Id);

            builder.Property(sidd => sidd.SGK_CPNK)
                .HasMaxLength(50);

            builder.Property(sidd => sidd.Note)
                .HasMaxLength(500);

            builder.Property(sidd => sidd.Quantity)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(sidd => sidd.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(sidd => sidd.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configure relationships
            builder.HasOne(sidd => sidd.SupplierImportDocument)
                .WithMany()
                .HasForeignKey(sidd => sidd.SupplierImportDocumentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sidd => sidd.Medicine)
                .WithMany()
                .HasForeignKey(sidd => sidd.MedicineId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
