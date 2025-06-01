using Management.API.Dtos.DepartmentType;

namespace Management.API.Dtos.Department
{
    public class DepartmentDetailDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameInEnglish { get; set; } = string.Empty;
        public DepartmentTypeDetailDto DepartmentType { get; set; } = new DepartmentTypeDetailDto();
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}