namespace Inventory.Infrastructure.Data.Configurations.Medicines
{
    public class MedicinePriceConfiguration : IEntityTypeConfiguration<MedicinePrice>
    {
        public void Configure(EntityTypeBuilder<MedicinePrice> builder)
        {
            builder.HasKey(mp => mp.Id);

            builder.Property(mp => mp.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(mp => mp.Currency)
                .HasMaxLength(3);

            builder.Property(mp => mp.VatRate)
                .IsRequired();

            builder.Property(mp => mp.VatAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(mp => mp.OriginalPriceAfterVat)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(mp => mp.OriginalPriceBeforeVat)
                .HasPrecision(18, 2)
                .IsRequired();
        }
    }
}
