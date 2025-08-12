using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId
{
    public record GetVaccinationHistoryByPatientIdQuery(int PatientId) : IQuery<GetVaccinationHistoryByPatientIdResult>;

    public record VaccinationHistoryItem(
        int Id,
        int ReceptionId,
        string MedicineTypeName,
        string MedicineName,
        string DoseNumber,
        DateTime? VaccinationTestDate,
        DateTime VaccinationDate,
        bool VaccinationConfirmation,
        string DoctorName
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
