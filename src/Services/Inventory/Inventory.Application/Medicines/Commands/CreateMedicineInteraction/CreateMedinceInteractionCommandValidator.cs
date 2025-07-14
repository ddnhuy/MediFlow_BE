namespace Inventory.Application.Medicines.Commands.CreateMedicineInteraction
{
    public class CreateMedicineInteractionCommandValidator : AbstractValidator<CreateMedicineInteractionCommand>
    {
        public CreateMedicineInteractionCommandValidator()
        {
            RuleFor(x => x.MedicineId1)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.REQUIRED_FIRST_MEDICINE.ToString());

            RuleFor(x => x.MedicineId2)
                .GreaterThan(0)
                .WithMessage(ExceptionKey.REQUIRED_SECOND_MEDICINE.ToString());

            RuleFor(x => x.MedicineId2)
                .NotEqual(x => x.MedicineId1)
                .WithMessage(ExceptionKey.SAME_MEDICINE_INTERACTION.ToString());

            RuleFor(x => x.HarmfulEffects)
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_INTERACTION_EFFECT.ToString());
        }
    }
}
