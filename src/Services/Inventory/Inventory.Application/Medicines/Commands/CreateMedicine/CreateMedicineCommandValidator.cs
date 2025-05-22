namespace Inventory.Application.Medicines.Commands.CreateMedicine
{
    public class CreateMedicineCommandValidator : AbstractValidator<CreateMedicineCommand>
    {
        public CreateMedicineCommandValidator()
        {
            RuleFor(x => x.MedicineCode)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_CODE)
                .MaximumLength(20).WithMessage(ValidationStrings.MAX_LENGTH("Mã thuốc", 20));

            RuleFor(x => x.MedicineName)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_NAME)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Tên thuốc", 100));

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_UNIT)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Đơn vị tính", 50));

            RuleFor(x => x.Manufacturer)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MANUFACTURER)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Nhà sản xuất", 100));

            RuleFor(x => x.ActiveIngredient)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_ACTIVE_INGREDIENT)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Hoạt chất", 100));

            RuleFor(x => x.UsageInstructions)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_USAGE_INSTRUCTIONS)
                .MaximumLength(200).WithMessage(ValidationStrings.MAX_LENGTH("Hướng dẫn sử dụng", 200));

            RuleFor(x => x.Concentration)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_CONCENTRATION)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Nồng độ/Hàm lượng", 50));

            RuleFor(x => x.Indications)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_INDICATIONS)
                .MaximumLength(200).WithMessage(ValidationStrings.MAX_LENGTH("Chỉ định", 200));

            RuleFor(x => x.MedicineClassification)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_CLASSIFICATION)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Phân loại thuốc", 100));

            RuleFor(x => x.RouteOfAdministration)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_ROUTE_OF_ADMINISTRATION)
                .MaximumLength(100).WithMessage(ValidationStrings.MAX_LENGTH("Đường dùng", 100));

            RuleFor(x => x.NationalMedicineCode)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_NATIONAL_MEDICINE_CODE)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Mã thuốc quốc gia", 50));

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty().WithMessage(ValidationStrings.REQUIRED_REGISTRATION_NUMBER)
                .MaximumLength(50).WithMessage(ValidationStrings.MAX_LENGTH("Số đăng ký", 50));

            RuleFor(x => x.MedicineTypeId)
                .GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_VALID_MEDICINE_TYPE);

            RuleFor(x => x.VaccineTypeId)
                .GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_VALID_VACCINE_TYPE);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(ValidationStrings.MAX_LENGTH("Mô tả", 500));

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage(ValidationStrings.MAX_LENGTH("Ghi chú", 500));
        }
    }
}