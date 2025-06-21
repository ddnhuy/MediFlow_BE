using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId
{
    public record GetMedicineListForVaccinationByReceptionIdQuery(int ReceptionId) : IQuery<List<GetMedicineListForVaccinationByReceptionIdResult>>;

    public record GetMedicineListForVaccinationByReceptionIdResult(
        int MedicineId,
        string MedicineName
    );
}
