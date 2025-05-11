namespace HumanResource.Grpc.Models
{
    public class DepartmentType : BaseEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
