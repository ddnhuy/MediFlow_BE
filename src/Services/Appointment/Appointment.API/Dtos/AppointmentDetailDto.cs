namespace Appointment.API.Dtos
{
    public class AppointmentDetailDto : AppointmentSummaryDto
    {
        public PatientDto Patient { get; set; } = new PatientDto();
        public DateTime CreatedAt { get; set; }
        public ApplicationUserDto CreatedBy { get; set; } = new ApplicationUserDto();
        public DateTime LastUpdatedAt { get; set; }
        public ApplicationUserDto LastUpdatedBy { get; set; } = new ApplicationUserDto();
    }
}