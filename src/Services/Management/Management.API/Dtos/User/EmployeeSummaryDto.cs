namespace Management.API.Dtos.User
{
    public class EmployeeSummaryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSuspended { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
