using Management.API.Dtos.Department;

namespace Management.API.Dtos.User
{
    public class UserDetailDto : UserSummaryDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsCancelled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public IEnumerable<DepartmentSummaryDto> Departments { get; set; } = new List<DepartmentSummaryDto>();
        public string Address { get; set; } = string.Empty;
    }
}
