using FluentValidation;
using Inventory.Application.Suppliers.Commands.CreateSupplier;
using Inventory.Application.Suppliers.Commands.UpdateSupplier;

namespace Inventory.Application.ValidationHelper
{
    public static class SupplierValidatorBase
    {
        public static void AddSupplierRules<T>(IRuleBuilder<T, string> ruleForSupplierName,
                                              IRuleBuilder<T, string> ruleForPhone,
                                              IRuleBuilder<T, string> ruleForEmail,
                                              IRuleBuilder<T, string> ruleForTaxCode,
                                              IRuleBuilder<T, string> ruleForAddress,
                                              IRuleBuilder<T, string> ruleForContactPerson,
                                              IRuleBuilder<T, string> ruleForDirector,
                                              IRuleBuilder<T, DateOnly> ruleForExpiredDate,
                                              IRuleBuilder<T, List<CreateSupplierContractRequest>>? ruleForCreateContracts = null,
                                              IRuleBuilder<T, List<UpdateSupplierContractRequest>>? ruleForUpdateContracts = null)
        {
            ruleForSupplierName
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_NAME.ToString());

            ruleForPhone
                .NotEmpty()
                .Length(10, 15)
                .Matches(@"^\d+$")
                .WithMessage(ExceptionKey.INVALID_SUPPLIER_PHONE.ToString());

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

            ruleForExpiredDate
                .NotNull()
                .NotEmpty()
                .GreaterThan(DateOnly.FromDateTime(DateTime.Today))
                .WithMessage(ExceptionKey.INVALID_SUPPLIER_EXPIRED_DATE.ToString());

            if (ruleForCreateContracts != null)
            {
                ruleForCreateContracts
                    .NotNull().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString())
                    .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString());
            }

            if (ruleForUpdateContracts != null)
            {
                ruleForUpdateContracts
                    .NotNull().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString())
                    .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString());
            }                
        }

        public static void AddContractRules<T>(IRuleBuilder<T, Guid> ruleForContractId,
                                              IRuleBuilder<T, string> ruleForContractFileName)
        {
            ruleForContractId
                .NotNull()
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_CONTRACT_ID.ToString());

            ruleForContractFileName
                .NotNull()
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_CONTRACT_FILE_NAME.ToString());
        }
    }
}