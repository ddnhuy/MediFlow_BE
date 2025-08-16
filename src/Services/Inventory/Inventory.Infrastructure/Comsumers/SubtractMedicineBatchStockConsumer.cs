using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStock;
using BuildingBlocks.Strings;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Comsumers
{
    public class SubtractMedicineBatchStockConsumer : IConsumer<SubtractMedicineBatchStockRequest>
    {
        private readonly IApplicationDbContext _context;    
        private readonly ILogger<SubtractMedicineBatchStockConsumer> _logger;

        public SubtractMedicineBatchStockConsumer(IApplicationDbContext context, ILogger<SubtractMedicineBatchStockConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubtractMedicineBatchStockRequest> context)
        {
            var request = context.Message;
            _logger.LogInformation("Received subtract stock request for MedicineBatchId: {MedicineBatchId}, Quantity: {Quantity}, RequestId: {RequestId}",
                request.MedicineBatchId, request.Quantity, request.RequestId);

            try
            {
                var inventoryDetail = await _context.InventoryDetails
                    .Include(id => id.MedicineBatch)
                    .FirstOrDefaultAsync(id => id.MedicineBatchId == request.MedicineBatchId, context.CancellationToken);

                if (inventoryDetail == null)
                {
                    await context.RespondAsync(new SubtractMedicineBatchStockResponse
                    {
                        MedicineBatchId = request.MedicineBatchId,
                        Quantity = request.Quantity,
                        IsSuccess = false,
                        ErrorMessage = $"Inventory detail not found for batch id: {request.MedicineBatchId}",
                        RequestId = request.RequestId
                    });
                    return;
                }

                if (inventoryDetail.Quantity < request.Quantity)
                {
                    await context.RespondAsync(new SubtractMedicineBatchStockResponse
                    {
                        MedicineBatchId = request.MedicineBatchId,
                        Quantity = request.Quantity,
                        IsSuccess = false,
                        ErrorMessage = $"Not enough stock in batch {request.MedicineBatchId}. Current: {inventoryDetail.Quantity}, Requested: {request.Quantity}",
                        RequestId = request.RequestId
                    });
                    return;
                }

                // Subtract the quantity
                inventoryDetail.Quantity -= request.Quantity;
                _context.InventoryDetails.Update(inventoryDetail);

                // Add inventory history
                var medicineId = inventoryDetail.MedicineBatch!.MedicineId;
                var medicine = await _context.MedicinePrices
                    .Where(mp => mp.MedicineId == medicineId)
                    .FirstOrDefaultAsync(context.CancellationToken);
                var medicinePrice = medicine?.UnitPrice ?? 0;

                // Add inventory history
                var inventoryHistory = new InventoryHistory
                {
                    MedicineId = inventoryDetail.MedicineBatch!.MedicineId,
                    MedicineBatchId = request.MedicineBatchId,
                    WarehouseId = inventoryDetail.WarehouseId,
                    TransactionDate = DateTime.UtcNow,
                    TransactionType = InventoryTransactionType.EXPORT,
                    Quantity = request.Quantity,
                    UnitPrice = medicinePrice,
                    Description = $"Exported {request.Quantity} medicine(s) from batch {inventoryDetail.MedicineBatch.BatchNumber}"
                };

                await _context.InventoryHistories.AddAsync(inventoryHistory);

                await _context.SaveChangesAsync(context.CancellationToken);

                await context.RespondAsync(new SubtractMedicineBatchStockResponse
                {
                    MedicineBatchId = request.MedicineBatchId,
                    Quantity = request.Quantity,
                    IsSuccess = true,
                    RequestId = request.RequestId
                });

                _logger.LogInformation("Successfully subtracted {Quantity} from batch {MedicineBatchId}", request.Quantity, request.MedicineBatchId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subtracting stock for batch {MedicineBatchId}", request.MedicineBatchId);
                await context.RespondAsync(new SubtractMedicineBatchStockResponse
                {
                    MedicineBatchId = request.MedicineBatchId,
                    Quantity = request.Quantity,
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while subtracting stock",
                    RequestId = request.RequestId
                });
            }
        }
    }
}
