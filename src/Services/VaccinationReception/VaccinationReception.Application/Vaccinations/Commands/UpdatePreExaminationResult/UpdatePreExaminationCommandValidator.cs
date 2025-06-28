using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdatePreExaminationResult
{
    public class UpdatePreExaminationCommandValidator : AbstractValidator<UpdatePreExaminationCommand>
    {
        public UpdatePreExaminationCommandValidator()
        {
            RuleFor(x => x.ReceptionVaccinationId)
                .GreaterThan(0).WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_ID.ToString());
            RuleFor(x => x.TestEntryResult)
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_TEST_ENTRY_RESULT.ToString());
        }
    }
}
