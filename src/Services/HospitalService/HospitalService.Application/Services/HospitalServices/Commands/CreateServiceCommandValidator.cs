using BuildingBlocks.Strings;
using FluentValidation;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
    {
        public CreateServiceCommandValidator()
        {
            RuleFor(x => x.ServiceCode)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_SERVICE_CODE.ToString())
                .MaximumLength(50)
                .WithMessage(ExceptionKey.SERVICE_CODE_MAX_LENGTH.ToString())
                .Matches("^[A-Za-z0-9-_]+$")
                .WithMessage(ExceptionKey.INVALID_SERVICE_CODE_FORMAT.ToString());

            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_SERVICE_NAME.ToString())
                .MaximumLength(200)
                .WithMessage(ExceptionKey.SERVICE_NAME_MAX_LENGTH.ToString())
                .MinimumLength(3)
                .WithMessage(ExceptionKey.SERVICE_NAME_MIN_LENGTH.ToString());

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_PRICE.ToString())
                .LessThanOrEqualTo(1000000000)
                .WithMessage(ExceptionKey.SERVICE_PRICE_TOO_LARGE.ToString());
        }
    }
}
