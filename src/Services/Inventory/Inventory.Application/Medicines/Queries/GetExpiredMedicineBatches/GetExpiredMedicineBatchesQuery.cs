namespace Inventory.Application.Medicines.Queries.GetExpiredMedicineBatches
{
    public record GetExpiredMedicineBatchesQuery(PaginationRequest PaginationRequest, string? SearchTerm = null) : IQuery<GetExpiredMedicineBatchesResult>;
    public record GetExpiredMedicineBatchesResult(PaginatedResult<ExpiredMedicineBatchDto> ExpiredBatches);
}
