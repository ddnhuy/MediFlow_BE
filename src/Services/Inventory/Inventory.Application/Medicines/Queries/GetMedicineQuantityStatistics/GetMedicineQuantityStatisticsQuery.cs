namespace Inventory.Application.Medicines.Queries.GetMedicineQuantityStatistics
{
    public record GetMedicineQuantityStatisticsQuery(PaginationRequest PaginationRequest, string? searchTerm = null) : IQuery<GetMedicineQuantityStatisticsResult>;
    public record GetMedicineQuantityStatisticsResult(PaginatedResult<MedicineQuantityStatisticsDto> Statistics);
}
