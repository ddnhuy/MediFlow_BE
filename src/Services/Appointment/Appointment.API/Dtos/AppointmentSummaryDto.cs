namespace Appointment.API.Dtos
{
    public class AppointmentSummaryDto
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string VaccineName { get; set; } = string.Empty;
        public string? Dose { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsCancelled { get; set; }
    }
}
