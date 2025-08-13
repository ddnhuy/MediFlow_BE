using BuildingBlocks.CQRS;
using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public record CloseReceptionWithIssueCommand(
        int ReceptionId,
        string IssueNote,
        DateTime? ReScheduleDate = null
    ) : ICommand<CloseReceptionWithIssueResult>;

    public record CloseReceptionWithIssueResult(
        bool IsSuccess
    );

    public class CloseReceptionWithIssueCommandValidator : AbstractValidator<CloseReceptionWithIssueCommand>
    {
        public CloseReceptionWithIssueCommandValidator()
        {
            RuleFor(x => x.ReceptionId)
                .GreaterThan(0).WithMessage(ExceptionKey.NOT_FOUND_RECEPTION.ToString());
            RuleFor(x => x.IssueNote)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_ISSUE_NOTE.ToString());
        }
    }
}
