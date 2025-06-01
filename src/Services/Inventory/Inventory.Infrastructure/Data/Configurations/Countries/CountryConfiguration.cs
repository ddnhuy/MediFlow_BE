namespace Inventory.Infrastructure.Data.Configurations.Countries
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CountryName)
                .IsRequired();

            builder.HasIndex(x => x.CountryName);
        }
    }
}
