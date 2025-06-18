using BuildingBlocks.Strings.Enums;

namespace Appointment.API.Models
{
    public class Appointment : BaseEntity
    {
        public int PatientId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string PatientEmail { get; set; } = default!;
        public string? PatientPhoneNumber { get; set; } = default!;
        public string? Note { get; set; }
    }
}
