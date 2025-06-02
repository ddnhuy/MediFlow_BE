namespace Inventory.Application.ValidationHelper
{
    public static class MedicinePriceValidatorBase
    {
        public static void AddMedicinePriceRules<T>(
            IRuleBuilder<T, int> ruleForMedicineId,
            IRuleBuilder<T, decimal> ruleForUnitPrice,
            IRuleBuilder<T, string> ruleForCurrency,
            IRuleBuilder<T, double> ruleForVatRate,
            IRuleBuilder<T, decimal> ruleForVatAmount,
            IRuleBuilder<T, decimal> ruleForOriginalPriceAfterVat,
            IRuleBuilder<T, decimal> ruleForOriginalPriceBeforeVat)
        {
            ruleForMedicineId
                .GreaterThan(0)
                .WithMessage(ValidationStrings.REQUIRED_MEDICINE_ID);

            ruleForUnitPrice
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_UNIT_PRICE)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationStrings.INVALID_UNIT_PRICE);

            ruleForCurrency
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_CURRENCY)
                .MaximumLength(3)
                .WithMessage(ValidationStrings.INVALID_CURRENCY_FORMAT);

            ruleForVatRate
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_VAT_RATE)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationStrings.INVALID_VAT_RATE);

            ruleForVatAmount
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_VAT_AMOUNT)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationStrings.INVALID_VAT_AMOUNT);

            ruleForOriginalPriceAfterVat
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_PRICE_AFTER_VAT)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationStrings.INVALID_PRICE_AFTER_VAT);

            ruleForOriginalPriceBeforeVat
                .NotEmpty()
                .WithMessage(ValidationStrings.REQUIRED_PRICE_BEFORE_VAT)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ValidationStrings.INVALID_PRICE_BEFORE_VAT);
        }
    }
}
