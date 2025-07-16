using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationTodayConfirmed
{
    public record ConfirmVaccinationTodayCommand(int ReceptionId) : ICommand<ConfirmVaccinationTodayResult>;
    public record ConfirmVaccinationTodayResult(bool IsSuccess);
}
