using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Comsumers
{
    public class GetNearestExpiryMedicineBatchConsumer : IConsumer<GetNearestExpiryMedicineBatchRequest>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetNearestExpiryMedicineBatchConsumer> _logger;

        public GetNearestExpiryMedicineBatchConsumer(
            IApplicationDbContext context,
            ILogger<GetNearestExpiryMedicineBatchConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetNearestExpiryMedicineBatchRequest> context)
        {
            var request = context.Message;

            _logger.LogInformation(
                "Received nearest expiry batch request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                request.MedicineId, request.RequestId);

            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                var query = from mb in _context.MedicineBatches
                            join id in _context.InventoryDetails on mb.Id equals id.MedicineBatchId
                            where mb.MedicineId == request.MedicineId
                                  && mb.ExpiryDate > today
                                  && id.Quantity > 0
                            orderby mb.ExpiryDate
                            select new {
                                mb.Id, 
                                mb.BatchNumber, 
                                mb.MedicineId,
                                mb.Medicine!.MedicineName,
                                mb.ExpiryDate 
                            };

                var result = await query.FirstOrDefaultAsync(context.CancellationToken);

                if (result is null)
                {
                    var errorResponse = new GetNearestExpiryMedicineBatchResponse
                    {
                        MedicineId = request.MedicineId,
                        RequestId = request.RequestId,
                        RequestedAt = request.RequestedAt,
                        IsSuccess = false,
                        ErrorMessage = $"No suitable medicine batch found for MedicineId: {request.MedicineId}"
                    };

                    await context.RespondAsync(errorResponse);
                    _logger.LogWarning("No suitable medicine batch found for MedicineId: {MedicineId}", request.MedicineId);
                    return;
                }

                var response = new GetNearestExpiryMedicineBatchResponse
                {
                    MedicineBatchId = result.Id,
                    MedicineBatchNumber = result.BatchNumber,
                    MedicineId = request.MedicineId,
                    MedicineName = result.MedicineName,
                    ExpiryDate = result.ExpiryDate,
                    RequestId = request.RequestId,
                    RequestedAt = request.RequestedAt,
                    IsSuccess = true
                };

                await context.RespondAsync(response);

                _logger.LogInformation(
                    "Successfully responded with nearest expiry batch for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing nearest expiry batch request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);

                var errorResponse = new GetNearestExpiryMedicineBatchResponse
                {
                    MedicineId = request.MedicineId,
                    RequestId = request.RequestId,
                    RequestedAt = request.RequestedAt,
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while retrieving nearest expiry medicine batch"
                };

                await context.RespondAsync(errorResponse);
            }
        }
    }
}
