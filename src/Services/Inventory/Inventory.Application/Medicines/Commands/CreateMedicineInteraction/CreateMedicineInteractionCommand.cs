namespace Inventory.Application.Medicines.Commands.CreateMedicineInteraction
{
    public record CreateMedicineInteractionCommand(
        int MedicineId1,
        int MedicineId2,
        string HarmfulEffects,
        string Mechanism,
        string PreventiveActions,
        string ReferenceInfo,
        string Notes) : ICommand<CreateMedicineInteractionResult>;

    public record CreateMedicineInteractionResult(int Id);
}
