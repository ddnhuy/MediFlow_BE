using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Workers.Email.Configurations;

namespace Workers.Email.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);
    }

    public sealed class EmailSender(
        ILogger<EmailSender> logger,
        IOptions<MailSettings> mailSettingsOptions)
        : IEmailSender
    {
        public async Task SendAsync(string to, string subject, string body)
        {
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mailSettingsOptions.Value.SenderName, mailSettingsOptions.Value.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = body };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(mailSettingsOptions.Value.Server, mailSettingsOptions.Value.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(mailSettingsOptions.Value.SenderEmail, mailSettingsOptions.Value.SenderPassword);
                await client.SendAsync(message);

                logger.LogInformation("Send an email with subject '{Subject}' to '{Email}' successfully!", subject, to);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
