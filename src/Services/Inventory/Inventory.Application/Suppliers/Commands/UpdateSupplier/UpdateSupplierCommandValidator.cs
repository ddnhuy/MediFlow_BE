namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.INVALID_DATA.ToString());

            SupplierValidatorBase.AddSupplierRules(
                ruleForSupplierName: RuleFor(x => x.SupplierName),
                ruleForEmail: RuleFor(x => x.Email),
                ruleForPhone: RuleFor(x => x.Phone),
                ruleForContactPerson: RuleFor(x => x.ContactPerson),
                ruleForTaxCode: RuleFor(x => x.TaxCode),
                ruleForAddress: RuleFor(x => x.Address),
                ruleForUpdateContracts: RuleFor(x => x.Contracts),
                ruleForDirector: RuleFor(x => x.Director),
                ruleForExpiredDate: RuleFor(x => x.ExpiredDate)
            );

            RuleFor(x => x.Contracts)
                .NotNull().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString())
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString());

            RuleForEach(x => x.Contracts).ChildRules(contract =>
            {
                SupplierValidatorBase.AddContractRules(
                    contract.RuleFor(x => x.Id),
                    contract.RuleFor(x => x.FileName)
                );
            });

            RuleFor(x => x.Contracts)
                .Must(contracts => contracts == null || contracts.Select(c => c.Id).Distinct().Count() == contracts.Count)
                .WithMessage(ExceptionKey.DUPLICATE_CONTRACT_ID.ToString());
        }
    }
}
