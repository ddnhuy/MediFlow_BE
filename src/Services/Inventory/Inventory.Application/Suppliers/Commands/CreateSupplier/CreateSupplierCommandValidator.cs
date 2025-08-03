namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            SupplierValidatorBase.AddSupplierRules(
                RuleFor(x => x.SupplierName),
                RuleFor(x => x.Phone),
                RuleFor(x => x.Email),
                RuleFor(x => x.TaxCode),
                RuleFor(x => x.Address),
                RuleFor(x => x.ContactPerson),
                RuleFor(x => x.Director),
                RuleFor(x => x.ExpiredDate),
                RuleFor(x => x.Contracts)
            );

            RuleFor(x => x.Contracts)
               .NotNull().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString())
               .NotEmpty().WithMessage(ExceptionKey.REQUIRED_SUPPLIER_CONTRACTS.ToString());

            RuleForEach(x => x.Contracts).ChildRules(contract =>
            {
                SupplierValidatorBase.AddContractRules(
                    ruleForContractFileName: contract.RuleFor(x => x.FileName),
                    ruleForContractId: contract.RuleFor(x => x.Id)
                );
            });

            // Check for duplicate contract IDs in the request
            RuleFor(x => x.Contracts)
                .Must(contracts => contracts == null || contracts.Count == contracts.Select(c => c.Id).Distinct().Count())
                .WithMessage(ExceptionKey.DUPLICATE_CONTRACT_ID.ToString());


        }
    }
}
