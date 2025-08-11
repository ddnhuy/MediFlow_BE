namespace Inventory.Application.Medicines.Queries.GetMedicineBatchesByMedicineId
{
    public record GetMedicineBatchesByMedicineIdQuery(int medicineId, PaginationRequest PaginationRequest, string? batchNumber = null) : IQuery<GetMedicineBatchesByMedicineIdResult>;

    public record GetMedicineBatchesByMedicineIdResult(PaginatedResult<MedicineBatchDTO> PaginatedResult);
}
