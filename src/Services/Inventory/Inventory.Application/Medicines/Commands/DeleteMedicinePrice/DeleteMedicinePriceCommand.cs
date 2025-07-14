namespace Inventory.Application.Medicines.Commands.DeleteMedicinePrice
{
    public record DeleteMedicinePriceCommand(int Id) : ICommand<DeleteMedicinePriceResult>;
    public record DeleteMedicinePriceResult(bool IsSuccess);
}
