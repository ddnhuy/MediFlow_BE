namespace Inventory.Application.Medicines.Queries.GetMedicineQuantityStatistics
{
    public class GetMedicineQuantityStatisticsQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineQuantityStatisticsQuery, GetMedicineQuantityStatisticsResult>
    {
        public async Task<GetMedicineQuantityStatisticsResult> Handle(
            GetMedicineQuantityStatisticsQuery request,
            CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var totalCount = await dbContext.Medicines
                .Where(m => !m.IsSuspended && !m.IsCancelled)
                .LongCountAsync(cancellationToken);

            var statistics = await dbContext.Medicines
                .Select(m => new MedicineQuantityStatisticsDto
                {
                    MedicineCode = m.MedicineCode ?? string.Empty,
                    MedicineName = m.MedicineName ?? string.Empty,
                    Unit = m.Unit ?? string.Empty,                   
                    NumberOfBatches = dbContext.MedicineBatches
                        .Where(mb => mb.MedicineId == m.Id 
                        && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow) 
                        && !mb.IsSuspended 
                        && !mb.IsCancelled)
                        .Count(),
                    TotalQuantity = dbContext.InventoryDetails
                        .Where(id => id.MedicineId == m.Id
                            && !id.IsSuspended
                            && !id.IsCancelled
                            && dbContext.MedicineBatches
                                .Any(mb => mb.Id == id.MedicineBatchId
                                    && !mb.IsSuspended
                                    && !mb.IsCancelled
                                    && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)))  
                        .Sum(id => id.Quantity)
                })
                .OrderByDescending(s => s.TotalQuantity)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var statisticDTO = statistics.Adapt<List<MedicineQuantityStatisticsDto>>();

            return new GetMedicineQuantityStatisticsResult(new PaginatedResult<MedicineQuantityStatisticsDto>(pageIndex, pageSize, totalCount, statisticDTO));
        }
    }
}
