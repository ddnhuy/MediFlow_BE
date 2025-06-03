namespace Inventory.Application.Medicines.Commands.DeleteMedicinePrice
{
    public class DeleteMedicinePriceCommandValidator : AbstractValidator<DeleteMedicinePriceCommand>
    {
        public DeleteMedicinePriceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Medicine price ID must be greater than 0");
        }
    }
}
