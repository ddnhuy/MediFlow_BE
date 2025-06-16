using BuildingBlocks.Strings;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Commands
{
    public class CreateDiseaseGroupCommandValidator : AbstractValidator<CreateDiseaseGroupCommand>
    {
        public CreateDiseaseGroupCommandValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_DISEASE_GROUP_NAME)
                .MaximumLength(200)
                .WithMessage(ValidationStrings.DISEASE_GROUP_NAME_MAX_LENGTH)
                .MinimumLength(3)
                .WithMessage(ValidationStrings.DISEASE_GROUP_NAME_MIN_LENGTH);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage(ValidationStrings.DISEASE_GROUP_DESCRIPTION_MAX_LENGTH)
                .When(x => x.Description != null);

            RuleForEach(x => x.ServiceIds)
                .GreaterThan(0)
                .WithMessage(ValidationStrings.INVALID_SERVICE_ID)
                .When(x => x.ServiceIds != null && x.ServiceIds.Any());
        }
    }
}
