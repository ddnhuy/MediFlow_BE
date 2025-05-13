namespace HumanResource.Grpc.Models
{
    public class Department : BaseEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int DepartmentTypeId { get; set; }
        public DepartmentType DepartmentType { get; set; } = default!;

        public IEnumerable<ApplicationUser> Users { get; set; } = default!;
    }
}
