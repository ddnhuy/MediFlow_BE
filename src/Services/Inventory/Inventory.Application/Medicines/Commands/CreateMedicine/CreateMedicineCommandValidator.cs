namespace Inventory.Application.Medicines.Commands.CreateMedicine
{
    public class CreateMedicineCommandValidator : AbstractValidator<CreateMedicineCommand>
    {
        public CreateMedicineCommandValidator()
        {
            MedicineValidatorBase.AddMedicineRules(
                RuleFor(x => x.MedicineCode),
                RuleFor(x => x.MedicineName),
                RuleFor(x => x.Unit),
                RuleFor(x => x.ActiveIngredient),
                RuleFor(x => x.UsageInstructions),
                RuleFor(x => x.Concentration),
                RuleFor(x => x.Indications),
                RuleFor(x => x.MedicineClassification),
                RuleFor(x => x.RouteOfAdministration),
                RuleFor(x => x.NationalMedicineCode),
                RuleFor(x => x.RegistrationNumber),
                RuleFor(x => x.MedicineTypeId),
                RuleFor(x => x.VaccineTypeId),
                RuleFor(x => x.Description),
                RuleFor(x => x.Note),
                RuleFor(x => x.IsRequiredTestingBeforeUse)
            );
        }
    }
}