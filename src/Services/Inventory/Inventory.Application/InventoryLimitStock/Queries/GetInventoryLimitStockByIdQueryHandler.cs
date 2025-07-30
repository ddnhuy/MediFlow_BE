using BuildingBlocks.Strings.Enums;
using Inventory.Application.Helpers;

namespace Inventory.Application.InventoryLimitStock
{
    public record GetInventoryLimitStockByIdQuery(int Id) : IQuery<GetInventoryLimitStockByIdResult>;
    public record GetInventoryLimitStockByIdResult(InventoryLimitStockDTO InventoryLimitStock);
    public class GetInventoryLimitStockByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetInventoryLimitStockByIdQuery, GetInventoryLimitStockByIdResult>
    {
        private const int CRITICAL_LOW = -100;
        private const int NORMAL = 0;

        public async Task<GetInventoryLimitStockByIdResult> Handle(GetInventoryLimitStockByIdQuery request, CancellationToken cancellationToken)
        {
            var ils = await dbContext.InventoryLimitStocks   
                .Include(x => x.Medicine)
                .Where(x => x.Id == request.Id && !x.IsCancelled)
                .Select(ils => new InventoryLimitStockDTO
                {
                    Id = ils.Id,
                    MedicineId = ils.MedicineId,
                    MedicineCode = ils.Medicine!.MedicineCode ?? string.Empty,
                    MedicineName = ils.Medicine.MedicineName ?? string.Empty,
                    Unit = ils.Medicine.Unit ?? string.Empty,
                    CurrentStock = dbContext.InventoryDetails
                        .Where(id => !id.IsSuspended
                            && !id.IsCancelled
                            && dbContext.MedicineBatches
                                .Any(mb => mb.Id == id.MedicineBatchId
                                    && mb.MedicineId == ils.MedicineId
                                    && !mb.IsSuspended
                                    && !mb.IsCancelled
                                    && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)))
                        .Sum(id => id.Quantity),
                    MinimalStockThreshold = ils.MinimalStockThreshold
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (ils == null)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_INVENTORY_LIMIT_STOCK_WITH_ID);

            ils.Difference = ils.CurrentStock - ils.MinimalStockThreshold;
            ils.InventoryLimitStockStatus = ils.Difference switch
            {
                < CRITICAL_LOW => InventoryLimitStockStatus.CriticalLow,
                < NORMAL => InventoryLimitStockStatus.Low,
                >= NORMAL => InventoryLimitStockStatus.Normal,
            };

            return new GetInventoryLimitStockByIdResult(ils);
        }
    }
}