namespace Inventory.Application.Medicines.Queries.GetExpiredMedicineBatches
{
    public class GetExpiredMedicineBatchesQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetExpiredMedicineBatchesQuery, GetExpiredMedicineBatchesResult>
    {
        public async Task<GetExpiredMedicineBatchesResult> Handle(
            GetExpiredMedicineBatchesQuery request,
            CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var totalCount = await dbContext.MedicineBatches
                .Where(mb => mb.ExpiryDate < today && !mb.IsSuspended && !mb.IsCancelled)
                .LongCountAsync(cancellationToken);

            var expiredBatches = await dbContext.MedicineBatches
            .Where(mb => mb.ExpiryDate < today && !mb.IsSuspended && !mb.IsCancelled)
            .Join(
                dbContext.Medicines,
                batch => batch.MedicineId,
                med => med.Id,
                (batch, med) => new
                {
                    Batch = batch,
                    Medicine = med
                })
            // Left join with InventoryDetails to get the total quantity for each batch
            .GroupJoin(
                dbContext.InventoryDetails.Where(id => !id.IsSuspended && !id.IsCancelled),
                batchMed => batchMed.Batch.Id,
                invDetail => invDetail.MedicineBatchId,
                (batchMed, inventoryDetails) => new
                {
                    batchMed.Batch,
                    batchMed.Medicine,
                    TotalQuantity = inventoryDetails.Sum(id => id.Quantity)
                })
            .Select(result => new ExpiredMedicineBatchDto
            {
                MedicineCode = result.Medicine.MedicineCode ?? string.Empty,
                MedicineName = result.Medicine.MedicineName ?? string.Empty,
                BatchNumber = result.Batch.BatchNumber,
                ExpiryDate = result.Batch.ExpiryDate,
                Unit = result.Medicine.Unit ?? string.Empty,
                CurrentQuantity = result.TotalQuantity
            })
            .OrderBy(b => b.ExpiryDate)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);



            return new GetExpiredMedicineBatchesResult(
                new PaginatedResult<ExpiredMedicineBatchDto>(pageIndex, pageSize, totalCount, expiredBatches));
        }
    }
}
