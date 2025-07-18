using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class Vaccination : BaseEntity
    {
        public int PatientId { get; set; }

        public int ReceptionVaccinationId { get; set; }
        public ReceptionVaccination? ReceptionVaccination { get; set; }

        public int MedicineBatchId { get; set; }
        public string? BatchNumber { get; set; }

        public int MedicineId { get; set; }
        public string? MedicineName { get; set; }

        public DateTime? VaccinationDate { get; set; }
        public string? Note { get; set; }

        public int DoctorId { get; set; }

        // PostVaccination properties
        public bool ObservationConfirmed { get; set; }
        public bool HasReaction { get; set; }
        public DateTime? ReactionDate { get; set; }
        public string? PostVaccinationResult { get; set; }
        public DateTime? PostVaccinationDate { get; set; }
        public bool HasFeverAbove39 { get; set; }
        public bool HasInjectionSiteReaction { get; set; }
        public bool HasOtherReaction { get; set; }
        public string? OtherReactionDescription { get; set; }

        public bool IsConfirmed { get; set; }
        public int DoseNumber { get; set; }
    }
}
