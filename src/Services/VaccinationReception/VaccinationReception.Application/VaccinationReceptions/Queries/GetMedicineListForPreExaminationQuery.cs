using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetMedicineListForPreExaminationQuery (int ReceptionId): IQuery<GetMedicineListForPreExaminationResult>;

    public record PreExaminationMedicineItem(
        int ReceptionVaccinationId,
        string PatientName,
        string VaccineName,
        bool IsConfirmed,
        DateTime VaccinationTestDate,
        string TestResultEntry,
        string DoctorName
    );

    public record GetMedicineListForPreExaminationResult(List<PreExaminationMedicineItem> PreExaminationMedicineItems);

}
