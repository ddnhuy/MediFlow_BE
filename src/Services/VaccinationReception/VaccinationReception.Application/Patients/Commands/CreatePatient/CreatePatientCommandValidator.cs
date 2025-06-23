using FluentValidation;
using VaccinationReception.Application.ValidationHelper;

namespace VaccinationReception.Application.Patients.Commands.CreatePatient
{
    public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
    {
        public CreatePatientCommandValidator()
        {
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
