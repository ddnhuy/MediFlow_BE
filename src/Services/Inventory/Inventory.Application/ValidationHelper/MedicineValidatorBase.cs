using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.ValidationHelper
{
    public static class MedicineValidatorBase
    {
        public static void AddMedicineRules<T>(IRuleBuilder<T, string> ruleForMedicineCode,
                                              IRuleBuilder<T, string> ruleForMedicineName,
                                              IRuleBuilder<T, string> ruleForUnit,
                                              IRuleBuilder<T, string> ruleForActiveIngredient,
                                              IRuleBuilder<T, string> ruleForUsageInstructions,
                                              IRuleBuilder<T, string> ruleForConcentration,
                                              IRuleBuilder<T, string> ruleForIndications,
                                              IRuleBuilder<T, string> ruleForMedicineClassification,
                                              IRuleBuilder<T, RouteOfAdministration> ruleForRouteOfAdministration,
                                              IRuleBuilder<T, string> ruleForNationalMedicineCode,
                                              IRuleBuilder<T, string> ruleForRegistrationNumber,
                                              IRuleBuilder<T, int> ruleForMedicineTypeId,
                                              IRuleBuilder<T, int> ruleForVaccineTypeId,
                                              IRuleBuilder<T, string> ruleForDescription,
                                              IRuleBuilder<T, string> ruleForNote,
                                              IRuleBuilder<T, bool?> ruleForIsRequiredTestingBeforeUse)
        {
            ruleForMedicineCode
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_CODE.ToString())
                .MaximumLength(20).WithMessage(ExceptionKey.INVALID_MEDICINE_CODE_MAX_LENGTH.ToString());

            ruleForMedicineName
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_NAME.ToString())
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_MEDICINE_NAME_MAX_LENGTH.ToString());

            ruleForUnit
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_UNIT.ToString())
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_MEDICINE_UNIT_MAX_LENGTH.ToString());

            ruleForActiveIngredient
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_ACTIVE_INGREDIENT.ToString())
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_ACTIVE_INGREDIENT_MAX_LENGTH.ToString());

            ruleForUsageInstructions
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_USAGE_INSTRUCTIONS.ToString())
                .MaximumLength(200).WithMessage(ExceptionKey.INVALID_USAGE_INSTRUCTIONS_MAX_LENGTH.ToString());

            ruleForConcentration
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_CONCENTRATION.ToString())
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_CONCENTRATION_MAX_LENGTH.ToString());

            ruleForIndications
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_INDICATIONS.ToString())
                .MaximumLength(200).WithMessage(ExceptionKey.INVALID_INDICATIONS_MAX_LENGTH.ToString());

            ruleForMedicineClassification
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_CLASSIFICATION.ToString())
                .MaximumLength(100).WithMessage(ExceptionKey.INVALID_MEDICINE_CLASSIFICATION_MAX_LENGTH.ToString());

            ruleForRouteOfAdministration
                .IsInEnum().WithMessage(ExceptionKey.REQUIRED_ROUTE_OF_ADMINISTRATION.ToString());

            ruleForNationalMedicineCode
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_NATIONAL_MEDICINE_CODE.ToString())
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_NATIONAL_MEDICINE_CODE_MAX_LENGTH.ToString());

            ruleForRegistrationNumber
                .NotEmpty().WithMessage(ExceptionKey.REQUIRED_REGISTRATION_NUMBER.ToString())
                .MaximumLength(50).WithMessage(ExceptionKey.INVALID_REGISTRATION_NUMBER_MAX_LENGTH.ToString());

            ruleForMedicineTypeId
                .GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_VALID_MEDICINE_TYPE.ToString());

            ruleForVaccineTypeId
                .GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_VALID_VACCINE_TYPE.ToString());

            ruleForDescription
                .MaximumLength(500).WithMessage(ExceptionKey.INVALID_MEDICINE_DESCRIPTION_MAX_LENGTH.ToString());

            ruleForNote
                .MaximumLength(500).WithMessage(ExceptionKey.INVALID_MEDICINE_NOTE_MAX_LENGTH.ToString());

            ruleForIsRequiredTestingBeforeUse
                .NotNull()
                .WithMessage(ExceptionKey.REQUIRED_IS_REQUIRED_TESTING_BEFORE_USE.ToString());
        }
    }
}
