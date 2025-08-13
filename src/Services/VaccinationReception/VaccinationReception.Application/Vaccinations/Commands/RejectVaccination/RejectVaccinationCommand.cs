using BuildingBlocks.CQRS;
using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.Vaccinations.Commands.RejectVaccination
{
    public record RejectVaccinationCommand(
        int ReceptionVaccinationId,
        string IssueNote
    ) : ICommand<RejectVaccinationResult>;

    public record RejectVaccinationResult(
        bool IsSuccess
    );

    public class RejectVaccinationCommandValidator : AbstractValidator<RejectVaccinationCommand>
    {
        public RejectVaccinationCommandValidator()
        {
            RuleFor(x => x.ReceptionVaccinationId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID.ToString());

            RuleFor(x => x.IssueNote)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_ISSUE_NOTE.ToString());
        }
    }
}
