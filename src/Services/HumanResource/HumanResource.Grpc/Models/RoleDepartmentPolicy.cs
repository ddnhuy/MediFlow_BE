namespace HumanResource.Grpc.Models
{
    public class RoleDepartmentPolicy
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int DepartmentId { get; set; }
        public int PolicyId { get; set; }

        public IdentityRole<int> Role { get; set; } = default!;
        public Department Department { get; set; } = default!;
        public Policy Policy { get; set; } = default!;
    }
}
