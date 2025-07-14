using BuildingBlocks.Strings.Enums;
using Inventory.Application.Helpers;

namespace Inventory.Application.InventoryLimitStock
{
    public record GetInventoryLimitStockQuery(PaginationRequest PaginationRequest) : IQuery<List<InventoryLimitStockDTO>>;

    public record GetInventoryLimitStockResult(PaginatedResult<InventoryLimitStockDTO> InventoryLimitStocks);

    public class GetInventoryLimitStockQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetInventoryLimitStockQuery, List<InventoryLimitStockDTO>>
    {
        private const int CRITICAL_LOW = -100;
        private const int NORMAL = 0;
        public async Task<List<InventoryLimitStockDTO>> Handle(GetInventoryLimitStockQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var totalCount = await dbContext.Medicines
                .Where(m => !m.IsSuspended && !m.IsCancelled)
                .LongCountAsync(cancellationToken);

            var inventoryLimitStocks = await dbContext.Medicines
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
                .ToListAsync(cancellationToken);

            inventoryLimitStocks = inventoryLimitStocks
                .Where(x => x.Id != 0)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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
            
            return inventoryLimitStocks;
        }
    }
}
