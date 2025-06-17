namespace Appointment.API.Dtos
{
    public class AppointmentSummaryDto
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}
