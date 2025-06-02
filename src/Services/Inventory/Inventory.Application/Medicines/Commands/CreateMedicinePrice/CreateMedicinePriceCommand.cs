namespace Inventory.Application.Medicines.Commands.CreateMedicinePrice
{
    public record CreateMedicinePriceCommand(
        int MedicineId,
        decimal UnitPrice,
        string Currency,
        double VatRate,
        decimal VatAmount,
        decimal OriginalPriceAfterVat,
        decimal OriginalPriceBeforeVat
    ) : ICommand<CreateMedicinePriceResult>;

    public record CreateMedicinePriceResult(int Id);
}
