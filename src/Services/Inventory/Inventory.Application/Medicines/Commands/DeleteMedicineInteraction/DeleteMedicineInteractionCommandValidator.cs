namespace Inventory.Application.Medicines.Commands.DeleteMedicineInteraction
{
    public class DeleteMedicineInteractionCommandValidator : AbstractValidator<DeleteMedicineInteractionCommand>
    {
        public DeleteMedicineInteractionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Vui lòng nhập ID");
        }
    }
}
