using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.DTOs.VaccinationDTOs
{
    public record VaccinationDetailDTO(
        int Id,
        int ReceptionId,
        string DoseNumber,
        DateTime? VaccinationTestDate,
        DateTime VaccinationDate,
        bool VaccinationConfirmation,
        string MedicineTypeName,
        string MedicineName,
        string DoctorName,
        // Post Vaccination
        bool ObservationConfirmed,
        bool HasReaction,
        DateTime? ReactionDate,
        string? PostVaccinationResult,
        DateTime? PostVaccinationDate,
        bool HasFeverAbove39,
        bool HasInjectionSiteReaction,
        bool HasOtherReaction,
        string? OtherReactionDescription,
        // Patient
        string PatientName,
        string PatientCode,
        string Gender,
        string PhoneNumber,
        string Ward,
        string District,
        string Province,
        string AddressDetail
    );
}
