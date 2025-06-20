using BuildingBlocks.Strings;
using FluentValidation;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateDiseaseGroupCommandValidator : AbstractValidator<CreateDiseaseGroupCommand>
    {
        public CreateDiseaseGroupCommandValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_DISEASE_GROUP_NAME.ToString())
                .MaximumLength(200)
                .WithMessage(ExceptionKey.DISEASE_GROUP_NAME_MAX_LENGTH.ToString())
                .MinimumLength(3)
                .WithMessage(ExceptionKey.DISEASE_GROUP_NAME_MIN_LENGTH.ToString());

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage(ExceptionKey.DISEASE_GROUP_DESCRIPTION_MAX_LENGTH.ToString())
                .When(x => x.Description != null);

            RuleForEach(x => x.ServiceIds)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_ID.ToString())
                .When(x => x.ServiceIds != null && x.ServiceIds.Any());
        }
    }
}
