using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class Examination : BaseEntity
    {
        public int? ServiceId { get; set; }
        public int? ReceptionId { get; set; }
        public Reception? Reception { get; set; }
        public string? RequestNumber { get; set; } = string.Empty;

        // Foreign key to Patient
        public int? PatientId { get; set; }
        public string? Diagnose { get; set; } = string.Empty;

        /**
         * The date and time when the examination was received.
         * This is the time when the patient arrives for the examination.
         */
        public DateTime? ReceptionTime { get; set; }

        /**
         * The date and time when the examination was executed.
         * This is the time when the examination process starts.
         */
        public DateTime? ExecutionTime { get; set; }

        // Foreign key to Technician (User)
        public int? PerformTechnicianId { get; set; }
        public string? PerformTechnicianName { get; set; } = string.Empty;

        /**
         * The date and time when the examination results are returned.
         * This is the time can be 30 minutes to 1.5 hour after the examination.
         */
        public DateTime? ReturnTime { get; set; }
        public SampleType? SampleType { get; set; }
        public SampleQualityLevel? SampleQuality { get; set; }

          // Foreign key to Doctor (User)
        public int? DoctorId { get; set; }
        public string? DoctorName { get; set; } = string.Empty;
        public string? Conclusion { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;

        // Navigation property for related services
        public virtual ICollection<ExaminationTestResult> ExaminationTestResults { get; set; } = new List<ExaminationTestResult>();
    }
}