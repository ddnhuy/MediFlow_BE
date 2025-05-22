namespace Inventory.Application.Medicines.Queries.GetMedicineInteractionByMedicineId
{
    public record GetMedicineInteractionsByMedicineIdQuery(int MedicineId) : IQuery<GetMedicineInteractionsByMedicineIdResult>;
    public record GetMedicineInteractionsByMedicineIdResult(List<MedicineInteractionDTO> MedicineInteractions);
}
