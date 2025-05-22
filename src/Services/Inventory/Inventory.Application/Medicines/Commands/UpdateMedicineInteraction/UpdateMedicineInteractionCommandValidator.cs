namespace Inventory.Application.Medicines.Commands.UpdateMedicineInteraction
{
    public class UpdateMedicineInteractionCommandValidator : AbstractValidator<UpdateMedicineInteractionCommand>
    {
        public UpdateMedicineInteractionCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Valid ID is required");

            RuleFor(x => x.MedicineId1)
                .GreaterThan(0)
                .WithMessage("First medicine must be specified");

            RuleFor(x => x.MedicineId2)
                .GreaterThan(0)
                .WithMessage("Second medicine must be specified");

            RuleFor(x => x.MedicineId2)
                .NotEqual(x => x.MedicineId1)
                .WithMessage("Cannot create an interaction between the same medicine");

            RuleFor(x => x.HarmfulEffects)
                .NotEmpty()
                .WithMessage("Harmful effects must be specified");
        }
    }
}
