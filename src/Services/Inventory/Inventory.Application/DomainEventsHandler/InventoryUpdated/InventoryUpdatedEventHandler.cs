using Inventory.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.DomainEventsHandler.InventoryUpdated
{
    public class InventoryUpdatedEventHandler(IApplicationDbContext dbContext, ILogger<InventoryUpdatedEventHandler> logger) : INotificationHandler<InventoryUpdatedEvent>
    {
        public async Task Handle(InventoryUpdatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Inventory updated: Batch Number {BatchNumber}, Quantity {Quantity}",
                notification.BatchNumber,
                notification.Quantity);

            // Check if inventory detail exists
            var inventoryDetail = await dbContext.InventoryDetails
                .FirstOrDefaultAsync(id => id.MedicineBatchId == notification.MedicineBatchId && id.WarehouseId == notification.WarehouseId, cancellationToken);

            if (inventoryDetail == null)
            {
                // Create new inventory detail
                inventoryDetail = new InventoryDetail
                {
                    MedicineBatchId = notification.MedicineBatchId,
                    WarehouseId = notification.WarehouseId,
                    Quantity = notification.Quantity,
                    CostPrice = notification.CostPrice
                };

                await dbContext.InventoryDetails.AddAsync(inventoryDetail);
            }
            else
            {
                // Update existing inventory detail
                inventoryDetail.Quantity += notification.Quantity;
                dbContext.InventoryDetails.Update(inventoryDetail);
            }

            // Create inventory history
            var inventoryHistory = new InventoryHistory
            {
                MedicineId = notification.MedicineId,
                MedicineBatchId = notification.MedicineBatchId,
                WarehouseId = notification.WarehouseId,
                TransactionDate = DateTime.UtcNow,
                TransactionType = InventoryTransactionType.IMPORT,
                Quantity = notification.Quantity,
                UnitPrice = notification.UnitPrice,
                Description = $"Imported medicine from batch {notification.BatchNumber}"
            };

            await dbContext.InventoryHistories.AddAsync(inventoryHistory);          

            await dbContext.SaveChangesAsync(cancellationToken);
        }

    }
}
