using BuildingBlocks.Strings;
using FluentValidation;
using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.Application.VaccinationReceptions.Validators
{
    public class CreatePatientReceptionCommandValidator : AbstractValidator<CreatePatientReceptionCommand>
    {
        public CreatePatientReceptionCommandValidator()
        {
            RuleFor(x => x.createPatientCommand)
                .NotNull()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_INFO.ToString());

            RuleFor(x => x.createReceptionDTO)
                .NotNull()
                .WithMessage(ExceptionKey.REQUIRED_VACCINATION_RECEPTION_INFO.ToString());

            RuleFor(x => x.createPatientCommand.Code)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_CODE.ToString())
                .MaximumLength(20)
                .WithMessage(ExceptionKey.INVALID_PATIENT_CODE_MAX_LENGTH.ToString());

            RuleFor(x => x.createPatientCommand.Name)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_NAME.ToString())
                .MaximumLength(100)
                .WithMessage(ExceptionKey.INVALID_PATIENT_NAME_MAX_LENGTH.ToString());

            RuleFor(x => x.createPatientCommand.Gender)
                .InclusiveBetween(0, 1)
                .WithMessage(ExceptionKey.INVALID_PATIENT_GENDER.ToString());

            RuleFor(x => x.createPatientCommand.Dob)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_DOB.ToString())
                .LessThan(DateTime.Now)
                .WithMessage(ExceptionKey.INVALID_PATIENT_DOB.ToString());

            RuleFor(x => x.createPatientCommand.PhoneNumber)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_PHONE.ToString())
                .Matches(@"^[0-9]{10,11}$")
                .WithMessage(ExceptionKey.INVALID_PATIENT_PHONE_FORMAT.ToString());

            RuleFor(x => x.createPatientCommand.Email)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_EMAIL.ToString())
                .EmailAddress()
                .WithMessage(ExceptionKey.INVALID_PATIENT_EMAIL.ToString());

            RuleFor(x => x.createPatientCommand.IdentityCard)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_IDENTITY_CARD.ToString())
                .Matches(@"^[0-9]{9,12}$")
                .WithMessage(ExceptionKey.INVALID_PATIENT_IDENTITY_CARD_FORMAT.ToString());

            RuleFor(x => x.createPatientCommand.AddressDetail)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_ADDRESS_DETAIL.ToString())
                .MaximumLength(200)
                .WithMessage(ExceptionKey.INVALID_PATIENT_ADDRESS_MAX_LENGTH.ToString());

            RuleFor(x => x.createPatientCommand.Province)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_PROVINCE.ToString())
                .MaximumLength(100)
                .WithMessage(ExceptionKey.INVALID_PATIENT_PROVINCE_MAX_LENGTH.ToString());

            RuleFor(x => x.createPatientCommand.District)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_DISTRICT.ToString())
                .MaximumLength(100)
                .WithMessage(ExceptionKey.INVALID_PATIENT_DISTRICT_MAX_LENGTH.ToString());

            RuleFor(x => x.createPatientCommand.Ward)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PATIENT_WARD.ToString())
                .MaximumLength(100)
                .WithMessage(ExceptionKey.INVALID_PATIENT_WARD_MAX_LENGTH.ToString());

            RuleFor(x => x.createReceptionDTO.ReceptionDate)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_VACCINATION_RECEPTION_DATE.ToString())
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage(ExceptionKey.INVALID_VACCINATION_RECEPTION_DATE.ToString());

            RuleFor(x => x.createReceptionDTO.ServiceTypeId)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_SERVICE_TYPE.ToString());

        }
    }
}