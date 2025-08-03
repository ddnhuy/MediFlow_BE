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
        decimal OriginalPriceBeforeVat,
        bool IsSuspended,
        bool IsCancelled
    ) : ICommand<UpdateMedicinePriceResult>;

    public record UpdateMedicinePriceResult(bool IsSuccess);
}
