using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Comsumers
{
    public class GetMedicineInteractionsConsumer : IConsumer<GetMedicineInteractionsRequest>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetMedicineInteractionsConsumer> _logger;

        public GetMedicineInteractionsConsumer(IApplicationDbContext context, ILogger<GetMedicineInteractionsConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetMedicineInteractionsRequest> context)
        {
            var request = context.Message;

            _logger.LogInformation("Received medicine interactions request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                request.MedicineId, request.RequestId);

            try
            {
                // Check if the medicine exists
                var medicine = await _context.Medicines
                    .FirstOrDefaultAsync(m => m.Id == request.MedicineId && !m.IsCancelled,
                        context.CancellationToken);

                if (medicine == null)
                {
                    var errorResponse = new GetMedicineInteractionsResponse
                    {
                        MedicineId = request.MedicineId,
                        RequestId = request.RequestId,
                        RequestedAt = request.RequestedAt,
                        IsSuccess = false,
                        ErrorMessage = $"Medicine with ID {request.MedicineId} not found"
                    };

                    await context.RespondAsync(errorResponse);
                    _logger.LogWarning("Medicine not found for MedicineId: {MedicineId}", request.MedicineId);
                    return;
                }

                // Get all interactions for this medicine
                var interactions = await _context.MedicineInteractions
                    .Include(mi => mi.Medicine1)
                    .Include(mi => mi.Medicine2)
                    .Where(mi => (mi.MedicineId1 == request.MedicineId || mi.MedicineId2 == request.MedicineId)
                                && !mi.IsCancelled)
                    .ToListAsync(context.CancellationToken);

                var interactionInfos = interactions.Select(mi => new MedicineInteractionInfo
                {
                    Id = mi.Id,
                    MedicineId1 = mi.MedicineId1,
                    MedicineId2 = mi.MedicineId2,
                    Medicine1Name = mi.Medicine1?.MedicineName,
                    Medicine2Name = mi.Medicine2?.MedicineName,
                    HarmfulEffects = mi.HarmfulEffects,
                    Mechanism = mi.Mechanism,
                    PreventiveActions = mi.PreventiveActions,
                    ReferenceInfo = mi.ReferenceInfo,
                    Notes = mi.Notes
                }).ToList();

                var response = new GetMedicineInteractionsResponse
                {
                    MedicineId = request.MedicineId,
                    MedicineName = medicine.MedicineName,
                    Interactions = interactionInfos,
                    RequestId = request.RequestId,
                    RequestedAt = request.RequestedAt,
                    IsSuccess = true
                };

                await context.RespondAsync(response);

                _logger.LogInformation("Successfully responded with medicine interactions for MedicineId: {MedicineId}, RequestId: {RequestId}, InteractionsCount: {InteractionsCount}",
                    request.MedicineId, request.RequestId, interactionInfos.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing medicine interactions request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);

                var errorResponse = new GetMedicineInteractionsResponse
                {
                    MedicineId = request.MedicineId,
                    RequestId = request.RequestId,
                    RequestedAt = request.RequestedAt,
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while retrieving medicine interactions"
                };

                await context.RespondAsync(errorResponse);
            }
        }
    }
}