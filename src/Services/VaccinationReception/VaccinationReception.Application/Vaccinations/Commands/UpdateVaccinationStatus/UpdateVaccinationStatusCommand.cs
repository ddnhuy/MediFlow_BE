using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationStatus
{
    public record UpdateVaccinationStatusCommand(int ReceptionVaccinationId, bool Status) : ICommand<UpdateVaccinationStatusCommandResult>;

    public record UpdateVaccinationStatusCommandResult(bool IsSuccess);
}
