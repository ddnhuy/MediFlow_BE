namespace Inventory.Application.Medicines.Commands.CreateMedicinePrice
{
    public class CreateMedicinePriceCommandValidator : AbstractValidator<CreateMedicinePriceCommand>
    {
        public CreateMedicinePriceCommandValidator()
        {
            MedicinePriceValidatorBase.AddMedicinePriceRules(
                RuleFor(x => x.MedicineId),
                RuleFor(x => x.UnitPrice),
                RuleFor(x => x.Currency),
                RuleFor(x => x.VatRate),
                RuleFor(x => x.VatAmount),
                RuleFor(x => x.OriginalPriceAfterVat),
                RuleFor(x => x.OriginalPriceBeforeVat)
            );
        }
    }
}
