namespace HumanResource.Grpc.Models
{
    public class ApplicationUser : IdentityUser<int>, IEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int LastUpdatedBy { get; set; }

        public IEnumerable<Department> Departments { get; set; } = default!;
    }
}
