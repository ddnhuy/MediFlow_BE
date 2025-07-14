using BuildingBlocks.Strings;
using FluentValidation;

namespace VaccinationReception.Application.ValidationHelper
{
    public static class PatientValidatorBase
    {
        public static void AddPatientRules<T>(
            IRuleBuilder<T, string> ruleForCode,
            IRuleBuilder<T, string> ruleForName,
            IRuleBuilder<T, int> ruleForGender,
            IRuleBuilder<T, DateTime> ruleForDOB,
            IRuleBuilder<T, string?> ruleForPhoneNumber,
            IRuleBuilder<T, string?> ruleForEmail,
            IRuleBuilder<T, string?> ruleForIdentityCard,
            IRuleBuilder<T, string?> ruleForAddressDetail,
            IRuleBuilder<T, string?> ruleForProvince,
            IRuleBuilder<T, string?> ruleForDistrict,
            IRuleBuilder<T, string?> ruleForWard)
        {
            ruleForCode
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PATIENT_CODE.ToString())
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_PATIENT_CODE_MAX_LENGTH.ToString());

            ruleForName
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PATIENT_NAME.ToString())
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_PATIENT_NAME_MAX_LENGTH.ToString());

            ruleForGender
                .InclusiveBetween(0, 1).WithMessage(ExceptionKey.REQUIRED_PATIENT_GENDER.ToString());

            ruleForDOB
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_PATIENT_DOB.ToString())
                .LessThanOrEqualTo(DateTime.Today).WithMessage(ExceptionKey.INVALID_PATIENT_DOB.ToString());

            ruleForPhoneNumber
                .MaximumLength(20).WithMessage(ExceptionKey.REQUIRED_PATIENT_PHONE.ToString())
                .Matches(@"^\+?[0-9]*$").When(x => !string.IsNullOrWhiteSpace(x?.ToString()))
                .WithMessage(ExceptionKey.INVALID_PATIENT_PHONE_MAX_LENGTH.ToString());

            ruleForEmail
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_PATIENT_EMAIL.ToString())
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x?.ToString()))
                .WithMessage(ExceptionKey.INVALID_PATIENT_EMAIL.ToString());

            ruleForIdentityCard
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_PATIENT_IDENTITY_CARD_MAX_LENGTH.ToString());

            ruleForAddressDetail
                .MaximumLength(200).WithMessage(ExceptionKey.INVALID_PATIENT_ADDRESS_MAX_LENGTH.ToString());

            ruleForProvince
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_PATIENT_PROVINCE_MAX_LENGTH.ToString());

            ruleForDistrict
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_PATIENT_DISTRICT_MAX_LENGTH.ToString());

            ruleForWard
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_PATIENT_WARD_MAX_LENGTH.ToString());
        }
    }
}