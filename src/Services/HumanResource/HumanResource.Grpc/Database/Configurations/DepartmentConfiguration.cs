using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HumanResource.Grpc.Database.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => x.Code);
            builder.HasOne(x => x.DepartmentType).WithMany().HasForeignKey(x => x.DepartmentTypeId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany<ApplicationUser>().WithMany(x => x.Departments);
        }
    }
}
