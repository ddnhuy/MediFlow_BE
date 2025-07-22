using FluentValidation;

namespace Inventory.Application.Medicines.Commands.UpdateMedicine
{
    public class UpdateMedicineCommandValidator : AbstractValidator<UpdateMedicineCommand>
    {
        public UpdateMedicineCommandValidator()
        {
            RuleFor(x => x.Id)
           .GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_MEDICINE_ID.ToString());

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
                RuleFor(x => x.VaccineTypeId),
                RuleFor(x => x.Description),
                ruleForIsRequiredTestingBeforeUse: RuleFor(x => x.IsRequiredTestingBeforeUse),
                ruleForNote: RuleFor(x => x.Note)
            );
        }
    }
}