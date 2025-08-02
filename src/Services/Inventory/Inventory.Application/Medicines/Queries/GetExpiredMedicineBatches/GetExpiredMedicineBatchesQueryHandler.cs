using BuildingBlocks.Strings.Enums;

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

            var baseQuery = dbContext.MedicineBatches
                .Where(mb => mb.ExpiryDate < today && mb.Status == MedicineBatchStatus.IsActive && !mb.IsSuspended && !mb.IsCancelled);

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(mb =>
                    mb.BatchNumber.ToLower().Contains(searchTerm) ||
                    dbContext.Medicines
                        .Where(m => m.Id == mb.MedicineId)
                        .Any(m => (m.MedicineCode != null && m.MedicineCode.ToLower().Contains(searchTerm)) ||
                                 (m.MedicineName != null && m.MedicineName.ToLower().Contains(searchTerm)))
                );
            }

            var totalCount = await baseQuery.LongCountAsync(cancellationToken);

            var expiredBatches = await baseQuery         
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
                MedicineId = result.Medicine.Id,    
                MedicineCode = result.Medicine.MedicineCode ?? string.Empty,
                MedicineName = result.Medicine.MedicineName ?? string.Empty,
                MedicineBatchId = result.Batch.Id,
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
