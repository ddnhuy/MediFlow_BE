namespace Inventory.Infrastructure.Data.Configurations.Suppliers
{
    public class SupplierContractConfiguration : IEntityTypeConfiguration<SupplierContract>
    {
        public void Configure(EntityTypeBuilder<SupplierContract> builder)
        {
            builder.HasKey(sc => sc.Id);
            builder.Property(sc => sc.FileName)
                .HasMaxLength(256);
            builder.HasOne(sc => sc.Supplier)
                .WithMany()
                .HasForeignKey(sc => sc.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(sc => sc.IsSuspended)
                .HasDefaultValue(false);
            builder.Property(sc => sc.IsCancelled)
                .HasDefaultValue(false);
        }
    }
}
