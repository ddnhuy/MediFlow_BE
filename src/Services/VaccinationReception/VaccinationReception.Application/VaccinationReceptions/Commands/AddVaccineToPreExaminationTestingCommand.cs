using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record AddVaccineToPreExaminationTestingCommand(
        int ReceptionVaccinationId
    ) : ICommand<AddVaccineToPreExaminationTestingResult>;

    public record AddVaccineToPreExaminationTestingResult(bool IsSuccess);
}
