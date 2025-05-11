using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HumanResource.Grpc.Database.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => x.UserName);
        }
    }
}
