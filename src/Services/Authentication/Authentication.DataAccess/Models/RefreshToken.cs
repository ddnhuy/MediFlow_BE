namespace Authentication.DataAccess.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = default!;
        public int UserId { get; set; }
        public DateTime ExpiresOnUtc { get; set; }
    }
}
