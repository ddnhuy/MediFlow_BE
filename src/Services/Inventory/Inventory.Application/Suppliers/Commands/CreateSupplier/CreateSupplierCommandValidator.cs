namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandValidator : AbstractValidator<CreateSupplierCommand>
    {
        public CreateSupplierCommandValidator()
        {
            SupplierValidatorBase.AddSupplierRules(
                RuleFor(x => x.SupplierCode),
                RuleFor(x => x.SupplierName),
                RuleFor(x => x.Phone),
                RuleFor(x => x.Fax),
                RuleFor(x => x.Email),
                RuleFor(x => x.TaxCode),
                RuleFor(x => x.Address),
                RuleFor(x => x.ContactPerson),
                RuleFor(x => x.Director)
            );
        }
    }
}
