namespace Inventory.Application.ValidationHelper
{
    public static class MedicineValidatorBase
    {
        public static void AddMedicineRules<T>(IRuleBuilder<T, string> ruleForMedicineCode,
                                              IRuleBuilder<T, string> ruleForMedicineName,
                                              IRuleBuilder<T, string> ruleForUnit,
                                              IRuleBuilder<T, string> ruleForManufacturer,
                                              IRuleBuilder<T, string> ruleForActiveIngredient,
                                              IRuleBuilder<T, string> ruleForUsageInstructions,
                                              IRuleBuilder<T, string> ruleForConcentration,
                                              IRuleBuilder<T, string> ruleForIndications,
                                              IRuleBuilder<T, string> ruleForMedicineClassification,
                                              IRuleBuilder<T, string> ruleForRouteOfAdministration,
                                              IRuleBuilder<T, string> ruleForNationalMedicineCode,
                                              IRuleBuilder<T, string> ruleForRegistrationNumber,
                                              IRuleBuilder<T, int> ruleForMedicineTypeId,
                                              IRuleBuilder<T, int> ruleForVaccineTypeId,
                                              IRuleBuilder<T, string> ruleForDescription,
                                              IRuleBuilder<T, string> ruleForNote)
        {
            ruleForMedicineCode
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_CODE)
                .MaximumLength(20).WithMessage(ValidationStrings.MAX_LENGTH("Mã thuốc", 20));

            ruleForMedicineName
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_NAME)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Tên thuốc", 100));

            ruleForUnit
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_UNIT)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Đơn vị tính", 50));

            ruleForManufacturer
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MANUFACTURER)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Nhà sản xuất", 100));

            ruleForActiveIngredient
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_ACTIVE_INGREDIENT)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Hoạt chất", 100));

            ruleForUsageInstructions
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_USAGE_INSTRUCTIONS)
                .MaximumLength(200).WithMessage(ValidationStrings.MAX_LENGTH("Hướng dẫn sử dụng", 200));

            ruleForConcentration
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_CONCENTRATION)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Nồng độ/Hàm lượng", 50));

            ruleForIndications
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_INDICATIONS)
                .MaximumLength(200).WithMessage(ValidationStrings.MAX_LENGTH("Chỉ định", 200));

            ruleForMedicineClassification
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_CLASSIFICATION)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Phân loại thuốc", 100));

            ruleForRouteOfAdministration
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_ROUTE_OF_ADMINISTRATION)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Đường dùng", 100));

            ruleForNationalMedicineCode
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_NATIONAL_MEDICINE_CODE)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Mã thuốc quốc gia", 50));

            ruleForRegistrationNumber
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_REGISTRATION_NUMBER)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Số đăng ký", 50));

            ruleForMedicineTypeId
                .GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_VALID_MEDICINE_TYPE);

            ruleForVaccineTypeId
                .GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_VALID_VACCINE_TYPE);

            ruleForDescription
                .MaximumLength(500).WithMessage(ValidationStrings.MAX_LENGTH("Mô tả", 500));

            ruleForNote
                .MaximumLength(500).WithMessage(ValidationStrings.MAX_LENGTH("Ghi chú", 500));
        }
    }
}
