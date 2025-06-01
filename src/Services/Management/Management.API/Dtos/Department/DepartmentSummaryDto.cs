using Management.API.Dtos.DepartmentType;

namespace Management.API.Dtos.Department
{
    public class DepartmentSummaryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameInEnglish { get; set; } = string.Empty;
        public DepartmentTypeSummaryDto DepartmentType { get; set; } = new DepartmentTypeSummaryDto();
        public bool IsSuspended { get; set; }
    }
}
