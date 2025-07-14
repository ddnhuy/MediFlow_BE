using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HumanResource.Grpc.Database.Configurations
{
    public class RoleDepartmentPolicyConfiguration : IEntityTypeConfiguration<RoleDepartmentPolicy>
    {
        public void Configure(EntityTypeBuilder<RoleDepartmentPolicy> builder)
        {
            builder.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Department).WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Policy).WithMany().HasForeignKey(x => x.PolicyId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
