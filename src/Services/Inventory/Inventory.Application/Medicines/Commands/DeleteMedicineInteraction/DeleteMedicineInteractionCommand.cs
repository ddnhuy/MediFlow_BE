namespace Inventory.Application.Medicines.Commands.DeleteMedicineInteraction
{
    public record DeleteMedicineInteractionCommand(int Id) : ICommand<DeleteMedicineInteractionResult>;
    public record DeleteMedicineInteractionResult(bool IsSuccess);
}
