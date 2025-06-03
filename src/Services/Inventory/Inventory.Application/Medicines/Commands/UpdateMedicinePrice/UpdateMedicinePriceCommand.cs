namespace Inventory.Application.Medicines.Commands.UpdateMedicinePrice
{
    public record UpdateMedicinePriceCommand(
        int Id,
        int MedicineId,
        decimal UnitPrice,
        string? Currency,
        double VatRate,
        decimal VatAmount,
        decimal OriginalPriceAfterVat,
        decimal OriginalPriceBeforeVat
    ) : ICommand<UpdateMedicinePriceResult>;

    public record UpdateMedicinePriceResult(bool IsSuccess);
}
