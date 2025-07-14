using BuildingBlocks.Strings;
using FluentValidation;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateServiceGroupCommandValidator : AbstractValidator<CreateServiceGroupCommand>
    {
        public CreateServiceGroupCommandValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_GROUP_NAME.ToString())
                .MaximumLength(200)
                .WithMessage(ExceptionKey.GROUP_NAME_MAX_LENGTH.ToString())
                .MinimumLength(3)
                .WithMessage(ExceptionKey.GROUP_NAME_MIN_LENGTH.ToString());

            RuleForEach(x => x.ServiceIds)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_ID.ToString())
                .When(x => x.ServiceIds != null && x.ServiceIds.Any());
        }
    }
}
