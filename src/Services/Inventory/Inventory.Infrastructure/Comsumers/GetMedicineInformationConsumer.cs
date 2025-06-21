using BuildingBlocks.Messaging.Contracts.Inventory;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Comsumers
{
    public class GetMedicineInformationConsumer : IConsumer<GetMedicineInformationRequest>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetMedicineInformationConsumer> _logger;

        public GetMedicineInformationConsumer(IApplicationDbContext context, ILogger<GetMedicineInformationConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<GetMedicineInformationRequest> context)
        {
            var request = context.Message;

            _logger.LogInformation("Received medicine information request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                request.MedicineId, request.RequestId);

            try
            {
                var medicine = await _context.Medicines
                    .Include(m => m.MedicineType)
                    .Include(m => m.VaccineType)
                    .FirstOrDefaultAsync(m => m.Id == request.MedicineId && !m.IsCancelled,
                        context.CancellationToken);

                var medicinePrice = await _context.MedicinePrices
                    .Where(mp => mp.MedicineId == request.MedicineId && !mp.IsCancelled)
                    .OrderByDescending(mp => mp.CreatedAt)
                    .FirstOrDefaultAsync(context.CancellationToken);

                if (medicine == null)
                {
                    var errorResponse = new GetMedicineInformationResponse
                    {
                        MedicineId = request.MedicineId,
                        RequestId = request.RequestId,
                        IsSuccess = false,
                        ErrorMessage = $"Medicine with ID {request.MedicineId} not found or is cancelled"
                    };

                    await context.RespondAsync(errorResponse);
                    _logger.LogWarning("Medicine not found for MedicineId: {MedicineId}", request.MedicineId);
                    return;
                }

                var response = new GetMedicineInformationResponse
                {
                    MedicineId = medicine.Id,
                    MedicineCode = medicine.MedicineCode,
                    MedicineName = medicine.MedicineName,
                    VaccineTypeName = medicine.VaccineType?.VaccineTypeName,
                    MedicineTypeName = medicine.MedicineType?.MedicineTypeName,
                    Unit = medicine.Unit,
                    UnitPrice = medicinePrice!.UnitPrice,
                    ActiveIngredient = medicine.ActiveIngredient,
                    UsageInstructions = medicine.UsageInstructions,
                    Concentration = medicine.Concentration,
                    Indications = medicine.Indications,
                    MedicineClassification = medicine.MedicineClassification,
                    RouteOfAdministration = medicine.RouteOfAdministration,
                    NationalMedicineCode = medicine.NationalMedicineCode,
                    Description = medicine.Description,
                    Note = medicine.Note,
                    RegistrationNumber = medicine.RegistrationNumber,
                    IsRequiredTestingBeforeUse = medicine.IsRequiredTestingBeforeUse,
                    MedicineTypeId = medicine.MedicineTypeId,
                    VaccineTypeId = medicine.VaccineTypeId,
                    IsSuspended = medicine.IsSuspended,
                    IsCancelled = medicine.IsCancelled,
                    CreatedAt = medicine.CreatedAt,
                    LastUpdatedAt = medicine.LastUpdatedAt,
                    RequestId = request.RequestId,
                    IsSuccess = true
                };

                await context.RespondAsync(response);

                _logger.LogInformation("Successfully responded with medicine information for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing medicine information request for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    request.MedicineId, request.RequestId);

                var errorResponse = new GetMedicineInformationResponse
                {
                    MedicineId = request.MedicineId,
                    RequestId = request.RequestId,
                    IsSuccess = false,
                    ErrorMessage = "An error occurred while retrieving medicine information"
                };

                await context.RespondAsync(errorResponse);
            }
        }
    }
}
