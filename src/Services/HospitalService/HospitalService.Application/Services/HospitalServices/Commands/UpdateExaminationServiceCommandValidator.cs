using BuildingBlocks.Strings;
using FluentValidation;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class UpdateExaminationServiceCommandValidator : AbstractValidator<UpdateExaminationServiceCommand>
    {
        public UpdateExaminationServiceCommandValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty()
                .WithMessage(ExceptionKey.SERVICE_NAME_MAX_LENGTH.ToString())
                .MinimumLength(3)
                .WithMessage(ExceptionKey.REQUIRED_SERVICE_NAME.ToString())
                .MaximumLength(200)
                .WithMessage(ExceptionKey.SERVICE_NAME_MIN_LENGTH.ToString());

            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_ID.ToString());

            RuleFor(x => x.ExaminationService)
                .IsInEnum()
                .WithMessage(ExceptionKey.SERVICE_IS_NOT_EXAMINATION_SERVICE.ToString());

            RuleFor(x => x.ServiceCode)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_SERVICE_CODE.ToString())
                .MaximumLength(50)
                .WithMessage(ExceptionKey.SERVICE_CODE_MAX_LENGTH.ToString())
                .Matches("^[A-Za-z0-9-_]+$")
                .WithMessage(ExceptionKey.INVALID_SERVICE_CODE_FORMAT.ToString());
                   
            RuleFor(x => x.DepartmentId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.REQUIRED_DEPARTMENT_ID.ToString());

            RuleFor(x => x.UnitPrice)
                .LessThanOrEqualTo(1000000000)
                .WithMessage(ExceptionKey.SERVICE_PRICE_TOO_LARGE.ToString())
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_PRICE.ToString());
     
            RuleFor(x => x.ServiceTestParameters)
                .NotNull()
                .Must(parameters => parameters != null && parameters.Any())
                .WithMessage(ExceptionKey.REQUIRED_SERVICE_TEST_PARAMETER.ToString());

            RuleForEach(x => x.ServiceTestParameters)
                .SetValidator(new ServiceTestParameterDtoValidator());
        }
    }
}