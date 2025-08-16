using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId
{
    public record GetVaccinationHistoryByPatientIdQuery(int PatientId) : IQuery<GetVaccinationHistoryByPatientIdResult>;

    public record VaccinationHistoryItem(
        int? Id,
        int ReceptionId,
        int ReceptionVaccinationId,
        string MedicineTypeName,
        string MedicineName,
        string DoseNumber,
        DateTime? VaccinationTestDate,
        DateTime? VaccinationDate,
        bool VaccinationConfirmation,
        string DoctorName,
        bool HasIssue,
        string? IssueNote = null,
        DateTime? IssueDate = null
    );

    public record GetVaccinationHistoryByPatientIdResult(
        string PatientCode,
        string PatientVaccinationCode,
        string PatientName,
        string Gender,
        string PhoneNumber,
        string AddressDetail,
        string Ward,
        string District,
        string Province,
        List<VaccinationHistoryItem> VaccinationHistoryItems
    );
}
