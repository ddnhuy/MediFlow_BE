namespace Inventory.Application.Medicines.Queries.GetMedicineQuantityStatistics
{
    public record GetMedicineQuantityStatisticsQuery(PaginationRequest PaginationRequest) : IQuery<GetMedicineQuantityStatisticsResult>;
    public record GetMedicineQuantityStatisticsResult(PaginatedResult<MedicineQuantityStatisticsDto> Statistics);
}
