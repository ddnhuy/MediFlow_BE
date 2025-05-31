namespace Inventory.Application.Medicines.Commands.ImportMedicineFromSupplier
{
    public class ImportMedicineFromSupplierCommandValidator : AbstractValidator<ImportMedicineFromSupplierCommand>
    {
        public ImportMedicineFromSupplierCommandValidator()
        {
            RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_WAREHOUSE_ID);
            RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_SUPPLIER_ID);
            RuleFor(x => x.ReceivedById).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_RECEIVER_ID);
            RuleFor(x => x.DocumentCode).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DOCUMENT_CODE);
            RuleFor(x => x.DocumentNumber).NotEmpty().WithMessage(ValidationStrings.REQUIRED_DOCUMENT_NUMBER);
            RuleFor(x => x.ImportDate).NotEmpty().WithMessage(ValidationStrings.REQUIRED_IMPORT_DATE);
            RuleFor(x => x.Details).NotEmpty().WithMessage(ValidationStrings.REQUIRED_MEDICINE_DETAIL);

            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_MEDICINE_ID);
                detail.RuleFor(x => x.BatchNumber).NotEmpty().WithMessage(ValidationStrings.REQUIRED_BATCH_NUMBER);
                detail.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(ValidationStrings.QUANTITY_GREATER_THAN_ZERO);
                detail.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage(ValidationStrings.UNIT_PRICE_NON_NEGATIVE);
                detail.RuleFor(x => x.ExpiryDate).NotEmpty().WithMessage(ValidationStrings.REQUIRED_EXPIRY_DATE)
                    .Must(expiryDate => expiryDate > DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage(ValidationStrings.EXPIRY_DATE_FUTURE);
                detail.RuleFor(x => x.ManufacturerId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_MANUFACTURER_ID);
                detail.RuleFor(x => x.CountryId).GreaterThan(0).WithMessage(ValidationStrings.REQUIRED_COUNTRY_ID);
            });
        }
    }
}
