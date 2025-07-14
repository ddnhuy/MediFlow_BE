using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.ValidationHelper;

namespace VaccinationReception.Application.Patients.Commands.UpdatePatient
{
    public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientCommandValidator()
        {
            RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("ID hợp lệ là bắt buộc");

            PatientValidatorBase.AddPatientRules(
                RuleFor(x => x.Code),
                RuleFor(x => x.Name),
                RuleFor(x => x.Gender),
                RuleFor(x => x.Dob),
                RuleFor(x => x.PhoneNumber),
                RuleFor(x => x.Email),
                RuleFor(x => x.IdentityCard),
                RuleFor(x => x.AddressDetail),
                RuleFor(x => x.Province),
                RuleFor(x => x.District),
                RuleFor(x => x.Ward)
            );
        }
    }
}