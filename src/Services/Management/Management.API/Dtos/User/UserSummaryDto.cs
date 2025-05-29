namespace Management.API.Dtos.User
{
    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsSuspended { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
