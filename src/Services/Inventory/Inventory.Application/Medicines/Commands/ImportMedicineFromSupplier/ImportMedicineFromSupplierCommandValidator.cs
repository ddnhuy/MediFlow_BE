namespace Inventory.Application.Medicines.Commands.ImportMedicineFromSupplier
{
    public class ImportMedicineFromSupplierCommandValidator : AbstractValidator<ImportMedicineFromSupplierCommand>
    {
        public ImportMedicineFromSupplierCommandValidator()
        {
            RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_WAREHOUSE_ID.ToString());
            RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_SUPPLIER_ID.ToString());
            RuleFor(x => x.ReceivedById).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_RECEIVER_ID.ToString());
            RuleFor(x => x.DocumentCode).NotEmpty().WithMessage(ExceptionKey.REQUIRED_DOCUMENT_CODE.ToString());
            RuleFor(x => x.DocumentNumber).NotEmpty().WithMessage(ExceptionKey.REQUIRED_DOCUMENT_NUMBER.ToString());
            RuleFor(x => x.ImportDate).NotEmpty().WithMessage(ExceptionKey.REQUIRED_IMPORT_DATE.ToString());
            RuleFor(x => x.Details).NotEmpty().WithMessage(ExceptionKey.REQUIRED_MEDICINE_DETAIL.ToString());

            RuleForEach(x => x.Details).ChildRules(detail =>
            {
                detail.RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_MEDICINE_ID.ToString());
                detail.RuleFor(x => x.BatchNumber).NotEmpty().WithMessage(ExceptionKey.REQUIRED_BATCH_NUMBER.ToString());
                detail.RuleFor(x => x.Quantity).GreaterThan(0).WithMessage(ExceptionKey.QUANTITY_GREATER_THAN_ZERO.ToString());
                detail.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0).WithMessage(ExceptionKey.UNIT_PRICE_NON_NEGATIVE.ToString());
                detail.RuleFor(x => x.ExpiryDate).NotEmpty().WithMessage(ExceptionKey.REQUIRED_EXPIRY_DATE.ToString())
                    .Must(expiryDate => expiryDate > DateOnly.FromDateTime(DateTime.Today))
                    .WithMessage(ExceptionKey.EXPIRY_DATE_FUTURE.ToString());
                detail.RuleFor(x => x.ManufacturerId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_MANUFACTURER_ID.ToString());
                detail.RuleFor(x => x.CountryId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_COUNTRY_ID.ToString());
            });
        }
    }
}
