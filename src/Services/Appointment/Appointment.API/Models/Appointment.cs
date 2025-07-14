namespace Appointment.API.Models
{
    public class Appointment : BaseEntity
    {
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
