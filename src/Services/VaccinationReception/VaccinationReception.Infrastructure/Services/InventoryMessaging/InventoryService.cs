using BuildingBlocks.Messaging.Contracts.Inventory;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using MassTransit;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;

namespace VaccinationReception.Infrastructure.Services.InventoryMessaging
{
    public class InventoryService : IInventoryService
    {
        private readonly IRequestClient<GetMedicineInformationRequest> _medicineInformationRequestClient;
        private readonly IRequestClient<GetNearestExpiryMedicineBatchRequest> _nearestExpiryMedicineBatchRequestClient;
        private readonly IRequestClient<GetMedicineInteractionsRequest> _medicineInteractionsRequestClient;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IRequestClient<GetMedicineInformationRequest> medicineInformationRequestClient,
            ILogger<InventoryService> logger,
            IRequestClient<GetNearestExpiryMedicineBatchRequest> nearestExpiryMedicineBatchRequestClient,
            IRequestClient<GetMedicineInteractionsRequest> medicineInteractionsRequestClient)
        {
            _medicineInformationRequestClient = medicineInformationRequestClient;
            _logger = logger;
            _nearestExpiryMedicineBatchRequestClient = nearestExpiryMedicineBatchRequestClient;
            _medicineInteractionsRequestClient = medicineInteractionsRequestClient;
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

                    var response = await _medicineInformationRequestClient.GetResponse<GetMedicineInformationResponse>(request, cancellationToken);

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

        public async Task<GetMedicineInteractionsResponse> GetMedicineInteractionsResponseAsync(int medicineId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetMedicineInteractionsRequest
                {
                    MedicineId = medicineId
                };

                _logger.LogInformation("Requesting medicine interactions for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    medicineId, request.RequestId);

                var response = await _medicineInteractionsRequestClient.GetResponse<GetMedicineInteractionsResponse>(request, cancellationToken);

                if (response.Message.IsSuccess)
                {
                    _logger.LogInformation("Successfully received medicine interactions for MedicineId: {MedicineId}, RequestId: {RequestId}",
                        medicineId, request.RequestId);
                }
                else
                {
                    _logger.LogWarning("Failed to get medicine interactions for MedicineId: {MedicineId}, Error: {Error}, RequestId: {RequestId}",
                        medicineId, response.Message.ErrorMessage, request.RequestId);
                }
                return response.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting medicine interactions for MedicineId: {MedicineId}", medicineId);
                throw;
            }
        }

        public async Task<GetNearestExpiryMedicineBatchResponse> GetNearestExpiryMedicineBatchAsync(int medicineId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetNearestExpiryMedicineBatchRequest
                {
                    MedicineId = medicineId
                };

                _logger.LogInformation("Requesting nearest expiry medicine batch for MedicineId: {MedicineId}, RequestId: {RequestId}",
                    medicineId, request.RequestId);

                var response = await _nearestExpiryMedicineBatchRequestClient.GetResponse<GetNearestExpiryMedicineBatchResponse>(request, cancellationToken);

                if (response.Message.IsSuccess)
                {
                    _logger.LogInformation("Successfully received nearest expiry medicine batch for MedicineId: {MedicineId}, RequestId: {RequestId}",
                        medicineId, request.RequestId);
                }
                else
                {
                    _logger.LogWarning("Failed to get nearest expiry medicine batch for MedicineId: {MedicineId}, Error: {Error}, RequestId: {RequestId}",
                        medicineId, response.Message.ErrorMessage, request.RequestId);
                }

                return response.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting nearest expiry medicine batch for MedicineId: {MedicineId}", medicineId);
                throw;
            }
        }
    }
}
