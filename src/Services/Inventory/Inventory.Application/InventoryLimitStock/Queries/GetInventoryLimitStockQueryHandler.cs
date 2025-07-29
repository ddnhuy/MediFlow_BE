using BuildingBlocks.Strings.Enums;
using Inventory.Application.Helpers;

namespace Inventory.Application.InventoryLimitStock
{
    public record GetInventoryLimitStockQuery(PaginationRequest PaginationRequest, string? searchKeyword = null) : IQuery<GetInventoryLimitStockResult>;

    public record GetInventoryLimitStockResult(PaginatedResult<InventoryLimitStockDTO> InventoryLimitStocks);

    public class GetInventoryLimitStockQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetInventoryLimitStockQuery, GetInventoryLimitStockResult>
    {
        private const int CRITICAL_LOW = -100;
        private const int NORMAL = 0;

        public async Task<GetInventoryLimitStockResult> Handle(GetInventoryLimitStockQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            // Build the base query for medicines with inventory limit stocks
            var baseQuery = dbContext.Medicines
                .Where(m => !m.IsSuspended && !m.IsCancelled)
                .Where(m => dbContext.InventoryLimitStocks
                    .Any(ils => ils.MedicineId == m.Id && !ils.IsSuspended && !ils.IsCancelled));

            // Apply search filter if searchKeyword is provided
            if (!string.IsNullOrWhiteSpace(request.searchKeyword))
            {
                var searchTerm = request.searchKeyword.Trim().ToLower();
                baseQuery = baseQuery.Where(m =>
                    (m.MedicineName != null && m.MedicineName.ToLower().Contains(searchTerm)) ||
                    (m.MedicineCode != null && m.MedicineCode.ToLower().Contains(searchTerm)));
            }

            // Get total count for pagination
            var totalCount = await baseQuery.LongCountAsync(cancellationToken);

            // Get paginated data
            var inventoryLimitStocks = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new InventoryLimitStockDTO
                {
                    Id = dbContext.InventoryLimitStocks
                        .Where(ils => ils.MedicineId == m.Id && !ils.IsSuspended && !ils.IsCancelled)
                        .Select(ils => ils.Id)
                        .FirstOrDefault(),
                    MedicineId = m.Id,
                    MedicineCode = m.MedicineCode ?? string.Empty,
                    MedicineName = m.MedicineName ?? string.Empty,
                    Unit = m.Unit ?? string.Empty,
                    CurrentStock = dbContext.InventoryDetails
                        .Where(id => !id.IsSuspended
                            && !id.IsCancelled
                            && dbContext.MedicineBatches
                                .Any(mb => mb.Id == id.MedicineBatchId
                                    && mb.MedicineId == m.Id
                                    && !mb.IsSuspended
                                    && !mb.IsCancelled
                                    && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)))
                        .Sum(id => id.Quantity),
                    MinimalStockThreshold = dbContext.InventoryLimitStocks
                        .Where(ils => ils.MedicineId == m.Id && !ils.IsSuspended && !ils.IsCancelled)
                        .Select(ils => ils.MinimalStockThreshold)
                        .FirstOrDefault(),
                })
                .OrderBy(m => m.MedicineId)
                .ToListAsync(cancellationToken);

            // Calculate status for each inventory limit stock
            foreach (var inventoryLimitStock in inventoryLimitStocks)
            {
                inventoryLimitStock.Difference = inventoryLimitStock.CurrentStock - inventoryLimitStock.MinimalStockThreshold;
                inventoryLimitStock.InventoryLimitStockStatus = inventoryLimitStock.Difference switch
                {
                    < CRITICAL_LOW => InventoryLimitStockStatus.CriticalLow,
                    < NORMAL => InventoryLimitStockStatus.Low,
                    >= NORMAL => InventoryLimitStockStatus.Normal,
                };
            }

            // Create paginated result
            var paginatedResult = new PaginatedResult<InventoryLimitStockDTO>(
                pageIndex,
                pageSize,
                totalCount,
                inventoryLimitStocks
            );

            return new GetInventoryLimitStockResult(paginatedResult);
        }
    }
}