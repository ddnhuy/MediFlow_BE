using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.CreateVaccination
{
    public record CreateVaccinationCommand(
        int PatientId,
        int ReceptionVaccinationId,
        int MedicineBatchId,
        string? BatchNumber,
        int MedicineId,
        string? MedicineName,
        string? Note,
        int DoctorId
    ) : ICommand<CreateVaccinationResponse>;
    public record CreateVaccinationResponse(int VaccinationId);
}
