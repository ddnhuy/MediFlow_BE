namespace Inventory.Application.Medicines.Queries.GetExpiredMedicineBatches
{
    public record GetExpiredMedicineBatchesQuery(PaginationRequest PaginationRequest) : IQuery<GetExpiredMedicineBatchesResult>;
    public record GetExpiredMedicineBatchesResult(PaginatedResult<ExpiredMedicineBatchDto> ExpiredBatches);
}
