namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id hợp lệ là bắt buộc");

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
