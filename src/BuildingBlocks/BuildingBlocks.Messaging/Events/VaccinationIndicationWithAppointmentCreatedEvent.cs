using BuildingBlocks.Strings.Enums;

namespace BuildingBlocks.Messaging.Events
{
    public record VaccinationIndicationWithAppointmentCreatedEvent : IntegrationEvent
    {
        public int UserId { get; set; }
        public int PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string PatientCode { get; set; } = default!;
        public string PatientFullName { get; set; } = default!;
        public DateTime PatientDOB { get; set; }
        public string PatientEmail { get; set; } = default!;
        public string? PatientPhoneNumber { get; set; } = default!;
        public string? VaccineName { get; set; } = default!;
        public string? Note { get; set; }
    }
}
