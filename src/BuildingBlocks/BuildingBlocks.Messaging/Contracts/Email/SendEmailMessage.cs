using BuildingBlocks.Messaging.Enums.BuildingBlocks.Messaging.Enums;

namespace BuildingBlocks.Messaging.Contracts.Email
{
    public record SendEmailMessage
    {
        public required string To { get; init; }
        public EmailSubjectCode SubjectCode { get; init; }
        public Dictionary<string, string> TemplateData { get; init; } = [];
    }
}
