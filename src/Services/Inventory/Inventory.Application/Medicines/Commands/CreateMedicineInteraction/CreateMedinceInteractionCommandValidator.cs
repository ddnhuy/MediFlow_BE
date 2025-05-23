namespace Inventory.Application.Medicines.Commands.CreateMedicineInteraction
{
    public class CreateMedicineInteractionCommandValidator : AbstractValidator<CreateMedicineInteractionCommand>
    {
        public CreateMedicineInteractionCommandValidator()
        {
            RuleFor(x => x.MedicineId1)
                .GreaterThan(0)
                .WithMessage(ValidationStrings.REQUIRED_FIRST_MEDICINE);

            RuleFor(x => x.MedicineId2)
                .GreaterThan(0)
                .WithMessage(ValidationStrings.REQUIRED_SECOND_MEDICINE);

            RuleFor(x => x.MedicineId2)
                .NotEqual(x => x.MedicineId1)
                .WithMessage(ValidationStrings.SAME_MEDICINE_INTERACTION);

            RuleFor(x => x.HarmfulEffects)
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_INTERACTION_EFFECT);
        }
    }
}
