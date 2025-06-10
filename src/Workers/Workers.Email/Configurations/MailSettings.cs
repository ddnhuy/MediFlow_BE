namespace Workers.Email.Configurations
{
    public class MailSettings
    {
        public required string Server { get; set; }
        public required int Port { get; set; }
        public required string SenderName { get; set; }
        public required string SenderEmail { get; set; }
        public required string SenderPassword { get; set; }
    }
}
