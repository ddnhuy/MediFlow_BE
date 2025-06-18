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
                .WithMessage(ExceptionKey.REQUIRED_MEDICINE_ID.ToString());

            ruleForUnitPrice
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_UNIT_PRICE.ToString())
                .GreaterThanOrEqualTo(0)
                .WithMessage(ExceptionKey.INVALID_UNIT_PRICE.ToString());

            ruleForCurrency
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_CURRENCY.ToString())
                .MaximumLength(3)
                .WithMessage(ExceptionKey.INVALID_CURRENCY_FORMAT.ToString());

            ruleForVatRate
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_VAT_RATE.ToString())
                .GreaterThanOrEqualTo(0)
                .WithMessage(ExceptionKey.INVALID_VAT_RATE.ToString());

            ruleForVatAmount
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_VAT_AMOUNT.ToString())
                .GreaterThanOrEqualTo(0)
                .WithMessage(ExceptionKey.INVALID_VAT_AMOUNT.ToString());

            ruleForOriginalPriceAfterVat
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PRICE_AFTER_VAT.ToString())
                .GreaterThanOrEqualTo(0)
                .WithMessage(ExceptionKey.INVALID_PRICE_AFTER_VAT.ToString());

            ruleForOriginalPriceBeforeVat
                .NotEmpty()
                .WithMessage(ExceptionKey.REQUIRED_PRICE_BEFORE_VAT.ToString())
                .GreaterThanOrEqualTo(0)
                .WithMessage(ExceptionKey.INVALID_PRICE_BEFORE_VAT.ToString());
        }
    }
}
