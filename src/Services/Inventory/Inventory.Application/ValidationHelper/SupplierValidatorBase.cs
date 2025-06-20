using FluentValidation;

namespace Inventory.Application.ValidationHelper
{
    public static class SupplierValidatorBase
    {
        public static void AddSupplierRules<T>(IRuleBuilder<T, string> ruleForSupplierCode,
                                              IRuleBuilder<T, string> ruleForSupplierName,
                                              IRuleBuilder<T, string> ruleForPhone,
                                              IRuleBuilder<T, string> ruleForFax,
                                              IRuleBuilder<T, string> ruleForEmail,
                                              IRuleBuilder<T, string> ruleForTaxCode,
                                              IRuleBuilder<T, string> ruleForAddress,
                                              IRuleBuilder<T, string> ruleForContactPerson,
                                              IRuleBuilder<T, string> ruleForDirector)
        {
            ruleForSupplierCode
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CODE.ToString());

            ruleForSupplierName
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_NAME.ToString());

            ruleForPhone
                .NotEmpty()
                .Length(10, 15)
                .Matches(@"^\d+$")
                .WithMessage(ExceptionKey.INVALID_SUPPLIER_PHONE.ToString());

            ruleForFax
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_FAX.ToString());

            ruleForEmail
                .NotEmpty()
                .EmailAddress()
                .WithMessage(ExceptionKey.INVALID_EMAIL.ToString());

            ruleForTaxCode
                .NotEmpty().WithMessage(ExceptionKey.INVALID_SUPPLIER_TAX_CODE.ToString());

            ruleForAddress
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_ADDRESS.ToString());

            ruleForContactPerson
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTACT_PERSON.ToString());

            ruleForDirector
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_DIRECTOR.ToString());

        }
    }
}
