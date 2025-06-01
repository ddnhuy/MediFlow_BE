namespace Inventory.Infrastructure.Data.Configurations.Suppliers
{
    public class SupplierImportDocumentConfiguration : IEntityTypeConfiguration<SupplierImportDocument>
    {
        public void Configure(EntityTypeBuilder<SupplierImportDocument> builder)
        {
            builder.HasKey(sid => sid.Id);

            builder.Property(sid => sid.DocumentCode)
                .HasMaxLength(50);

            builder.Property(sid => sid.DocumentNumber)
                .HasMaxLength(50);

            builder.Property(sid => sid.Note)
                .HasMaxLength(500);

            builder.Property(sid => sid.SupportingDocument)
                .HasMaxLength(255);

            // Configure relationships
            builder.HasOne(sid => sid.Warehouse)
                .WithMany()
                .HasForeignKey(sid => sid.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sid => sid.Supplier)
                .WithMany()
                .HasForeignKey(sid => sid.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
