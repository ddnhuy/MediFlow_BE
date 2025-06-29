using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdatePreExaminationResult
{
    public record UpdatePreExaminationCommand (int ReceptionVaccinationId, string TestEntryResult) : ICommand<UpdatePreExaminationResult>;

    public record UpdatePreExaminationResult(bool IsSucess);
}
