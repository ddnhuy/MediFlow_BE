namespace Inventory.Application.Medicines.Commands.DeleteMedicine
{
    public record DeleteMedicineCommand(int Id) : ICommand<DeleteMedicineResult>;
    public record DeleteMedicineResult(bool IsSuccess);
}
