using BuildingBlocks.Messaging.Contracts.Email;
using BuildingBlocks.Messaging.Extensions;
using MassTransit;
using Workers.Email.Services;

namespace Workers.Email.Consumers
{
    public class SendEmailConsumer(
        IEmailSender emailSender,
        IEmailTemplateRenderer templateRenderer) : IConsumer<SendEmailMessage>
    {
        public async Task Consume(ConsumeContext<SendEmailMessage> context)
        {
            var message = context.Message;

            string htmlBody = await templateRenderer.RenderAsync(
                message.SubjectCode.ToString(),
                message.TemplateData
            );

            await emailSender.SendAsync(message.To, message.SubjectCode.GetSubjectText(), htmlBody);
        }
    }
}
