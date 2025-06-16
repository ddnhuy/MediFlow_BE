using BuildingBlocks.Strings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
    {
        public CreateServiceCommandValidator()
        {
            RuleFor(x => x.ServiceCode)
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_SERVICE_CODE)
                .MaximumLength(50)
                .WithMessage(ValidationStrings.SERVICE_CODE_MAX_LENGTH)
                .Matches("^[A-Za-z0-9-_]+$")
                .WithMessage(ValidationStrings.INVALID_SERVICE_CODE_FORMAT);

            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_SERVICE_NAME)
                .MaximumLength(200)
                .WithMessage(ValidationStrings.SERVICE_NAME_MAX_LENGTH)
                .MinimumLength(3)
                .WithMessage(ValidationStrings.SERVICE_NAME_MIN_LENGTH);

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage(ValidationStrings.INVALID_SERVICE_PRICE)
                .LessThanOrEqualTo(1000000000)
                .WithMessage(ValidationStrings.SERVICE_PRICE_TOO_LARGE);
        }
    }
}
