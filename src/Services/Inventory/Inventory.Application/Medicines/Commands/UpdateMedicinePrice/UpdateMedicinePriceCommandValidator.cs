namespace Inventory.Application.Medicines.Commands.UpdateMedicinePrice
{
    public class UpdateMedicinePriceCommandValidator : AbstractValidator<UpdateMedicinePriceCommand>
    {
        public UpdateMedicinePriceCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Medicine price ID must be greater than 0");

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
