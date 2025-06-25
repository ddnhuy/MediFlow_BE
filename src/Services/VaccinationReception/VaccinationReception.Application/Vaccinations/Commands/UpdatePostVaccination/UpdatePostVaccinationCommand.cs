using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdatePostVaccination
{
    public record UpdatePostVaccinationCommand(
        int Id,
        bool ObservationConfirmed,
        bool HasReaction,
        DateTime? ReactionDate,
        string? PostVaccinationResult,
        DateTime? PostVaccinationDate,
        bool HasFeverAbove39,
        bool HasInjectionSiteReaction,
        bool HasOtherReaction,
        string? OtherReactionDescription
    ) : ICommand<UpdatePostVaccinationResult>;

    public record UpdatePostVaccinationResult(bool IsSuccess);
}
