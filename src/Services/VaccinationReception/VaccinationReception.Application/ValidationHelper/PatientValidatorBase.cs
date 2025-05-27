using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Const;

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
            IRuleBuilder<T, string?> ruleForIdentityCard,
            IRuleBuilder<T, string?> ruleForAddressDetail,
            IRuleBuilder<T, string?> ruleForProvince,
            IRuleBuilder<T, string?> ruleForDistrict,
            IRuleBuilder<T, string?> ruleForWard)
        {
            ruleForCode
                .NotEmpty().WithMessage(ValidationMessages.Code_Required)
                .MaximumLength(50).WithMessage(ValidationMessages.Code_MaxLength);

            ruleForName
                .NotEmpty().WithMessage(ValidationMessages.Name_Required)
                .MaximumLength(100).WithMessage(ValidationMessages.Name_MaxLength);

            ruleForGender
                .InclusiveBetween(0, 1).WithMessage(ValidationMessages.InvalidGender);

            ruleForDOB
                .NotEmpty().WithMessage(ValidationMessages.DOB_Required)
                .LessThanOrEqualTo(DateTime.Today).WithMessage(ValidationMessages.InvalidDate);

            ruleForPhoneNumber
                .MaximumLength(20).WithMessage(ValidationMessages.Phone_MaxLength)
                .Matches(@"^\+?[0-9]*$").When(x => !string.IsNullOrWhiteSpace(x?.ToString()))
                .WithMessage(ValidationMessages.Phone_Invalid);

            ruleForIdentityCard
                .MaximumLength(50).WithMessage(ValidationMessages.IdentityCard_MaxLength);

            ruleForAddressDetail
                .MaximumLength(200).WithMessage(ValidationMessages.Address_MaxLength);

            ruleForProvince
                .MaximumLength(100).WithMessage(ValidationMessages.Province_MaxLength);

            ruleForDistrict
                .MaximumLength(100).WithMessage(ValidationMessages.District_MaxLength);

            ruleForWard
                .MaximumLength(100).WithMessage(ValidationMessages.Ward_MaxLength);
        }
    }
}