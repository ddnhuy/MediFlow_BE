using BuildingBlocks.Messaging.Contracts.Inventory;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using MassTransit;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;

namespace VaccinationReception.Infrastructure.Services.InventoryMessaging
{
    public class InventoryService : IInventoryService
    {
        private readonly IRequestClient<GetMedicineInformationRequest> _requestClient;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IRequestClient<GetMedicineInformationRequest> requestClient,
            ILogger<InventoryService> logger)
        {
            _requestClient = requestClient;
            _logger = logger;
        }

        public async Task<List<GetMedicineInformationResponse>> GetMedicineInformationAsync(IEnumerable<int> medicineIdList, CancellationToken cancellationToken = default)
        {
            try
            {
                var responses = new List<GetMedicineInformationResponse>();

                foreach (var medicineId in medicineIdList)
                {
                    var request = new GetMedicineInformationRequest
                    {
                        MedicineId = medicineId
                    };

                    _logger.LogInformation("Requesting medicine information for MedicineId: {MedicineId}, RequestId: {RequestId}",
                        medicineId, request.RequestId);

                    var response = await _requestClient.GetResponse<GetMedicineInformationResponse>(request, cancellationToken);

                    if (response.Message.IsSuccess)
                    {
                        _logger.LogInformation("Successfully received medicine information for MedicineId: {MedicineId}, RequestId: {RequestId}",
                            medicineId, request.RequestId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to get medicine information for MedicineId: {MedicineId}, Error: {Error}, RequestId: {RequestId}",
                            medicineId, response.Message.ErrorMessage, request.RequestId);
                    }

                    responses.Add(response.Message);
                }

                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting medicine information for multiple medicines");
                throw;
            }
        }
    }
}
