namespace Inventory.Application.Medicines.Commands.UpdateMedicineInteraction
{
    public record UpdateMedicineInteractionCommand(
        int Id,
        int MedicineId1,
        int MedicineId2,
        string HarmfulEffects,
        string Mechanism,
        string PreventiveActions,
        string ReferenceInfo,
        string Notes,
        bool IsSuspended,
        bool IsCancelled) : ICommand<UpdateMedicineInteractionResult>;

    public record UpdateMedicineInteractionResult(bool IsSuccess);
}
