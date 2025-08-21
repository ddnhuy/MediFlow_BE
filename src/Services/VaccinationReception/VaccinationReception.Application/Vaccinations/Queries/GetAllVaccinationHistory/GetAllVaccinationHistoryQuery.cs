using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;

namespace VaccinationReception.Application.Vaccinations.Queries.GetAllVaccinationHistory
{
    public record GetAllVaccinationHistoryQuery(
        PaginationRequest PaginationRequest,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        string? SearchTerm = null
    ) : IQuery<PaginatedResult<AllVaccinationHistoryItem>>;

    public record AllVaccinationHistoryItem(
        int? Id,
        int ReceptionId,
        int ReceptionVaccinationId,
        int PatientId,
        string PatientCode,
        string PatientName,
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
}
