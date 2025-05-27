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
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_CODE);

            ruleForSupplierName
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_NAME);

            ruleForPhone
                .NotEmpty()
                .Length(10, 15)
                .Matches(@"^\d+$")
                .WithMessage(ValidationStrings.INVALID_SUPPLIER_PHONE);

            ruleForFax
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_FAX);

            ruleForEmail
                .NotEmpty()
                .EmailAddress()
                .WithMessage(ValidationStrings.INVALID_EMAIL);

            ruleForTaxCode
                .NotEmpty().WithMessage(ValidationStrings.INVALID_SUPPLIER_TAX_CODE);

            ruleForAddress
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_ADDRESS);

            ruleForContactPerson
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_CONTACT_PERSON);

            ruleForDirector
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_SUPPLIER_DIRECTOR);

        }
    }
}
