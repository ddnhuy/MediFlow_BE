namespace Authentication.DataAccess.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = default!;
        public int UserId { get; set; }
        public string Roles { get; set; } = default!;
        public DateTime ExpiresOnUtc { get; set; }
    }
}
