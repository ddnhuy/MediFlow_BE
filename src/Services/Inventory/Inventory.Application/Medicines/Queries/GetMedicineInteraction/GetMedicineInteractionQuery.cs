namespace Inventory.Application.Medicines.Queries.GetMedicineInteraction
{
    public record GetMedicineInteractionsQuery(PaginationRequest PaginationRequest) : IQuery<GetMedicineInteractionsResult>;
    public record GetMedicineInteractionsResult(PaginatedResult<MedicineInteractionDTO> MedicineInteractions);
}
