using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStockStatus;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Infrastructure.Comsumers
{
    public class CheckMedicineStockConsumer : IConsumer<CheckMedicineStockRequest>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CheckMedicineStockConsumer> _logger;

        public CheckMedicineStockConsumer(IApplicationDbContext context, ILogger<CheckMedicineStockConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CheckMedicineStockRequest> context)
        {
            var request = context.Message;
            _logger.LogInformation("Received stock check request for MedicineId: {MedicineId}, NumberOfMedicineWanted: {NumberOfMedicineWanted}, RequestId: {RequestId}",
                request.MedicineId, request.NumberOfMedicineWanted, request.RequestId);

            try
            {
                // Sum all available stock for the medicine
                var currentStock = await _context.InventoryDetails
                    .Include(id => id.MedicineBatch)
                    .Where(id => id.MedicineBatch!.MedicineId == request.MedicineId 
                    && id.MedicineBatch!.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)
                    && !id.MedicineBatch!.IsSuspended 
                    && !id.MedicineBatch!.IsCancelled)
                    .SumAsync(id => id.Quantity, context.CancellationToken);

                var isEnough = currentStock >= request.NumberOfMedicineWanted;
                var difference = currentStock - request.NumberOfMedicineWanted;

                var response = new CheckMedicineStockResponse
                {
                    MedicineId = request.MedicineId,
                    NumberOfMedicineWanted = request.NumberOfMedicineWanted,
                    CurrentStock = currentStock,
                    IsEnough = isEnough,
                    Difference = difference,
                    RequestId = request.RequestId,
                    IsSuccess = true
                };

                await context.RespondAsync(response);
                _logger.LogInformation("Responded to stock check for MedicineId: {MedicineId}, IsEnough: {IsEnough}, Difference: {Difference}",
                    request.MedicineId, isEnough, difference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock check request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);

                var errorResponse = new CheckMedicineStockResponse
                {
                    MedicineId = request.MedicineId,
                    NumberOfMedicineWanted = request.NumberOfMedicineWanted,
                    CurrentStock = 0,
                    IsEnough = false,
                    Difference = 0,
                    RequestId = request.RequestId,
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while checking stock"
                };

                await context.RespondAsync(errorResponse);
            }
        }
    }
}
