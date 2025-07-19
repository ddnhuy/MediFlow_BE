using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationStatus
{
    public record UpdateVaccinationStatusCommand(int VaccinationId, bool Status) : ICommand<UpdateVaccinationStatusCommandResult>;

    public record UpdateVaccinationStatusCommandResult(bool IsSuccess);
}
