using BuildingBlocks.Messaging.Contracts.Inventory;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStock;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineStockStatus;
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
        private readonly IRequestClient<CheckMedicineStockRequest> _checkMedicineStockRequestClient;
        private readonly IRequestClient<SubtractMedicineBatchStockRequest> _subtractMedicineBatchStockRequestClient;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(
            IRequestClient<GetMedicineInformationRequest> medicineInformationRequestClient,
            ILogger<InventoryService> logger,
            IRequestClient<GetNearestExpiryMedicineBatchRequest> nearestExpiryMedicineBatchRequestClient,
            IRequestClient<GetMedicineInteractionsRequest> medicineInteractionsRequestClient,
            IRequestClient<CheckMedicineStockRequest> checkMedicineStockRequestClient,
            IRequestClient<SubtractMedicineBatchStockRequest> subtractMedicineBatchStockRequestClient)
        {
            _medicineInformationRequestClient = medicineInformationRequestClient;
            _logger = logger;
            _nearestExpiryMedicineBatchRequestClient = nearestExpiryMedicineBatchRequestClient;
            _medicineInteractionsRequestClient = medicineInteractionsRequestClient;
            _checkMedicineStockRequestClient = checkMedicineStockRequestClient;
            _subtractMedicineBatchStockRequestClient = subtractMedicineBatchStockRequestClient;
        }

        public async Task<CheckMedicineStockResponse> CheckMedicineStockResponseAsync(int medicineId, int numberOfMedicineWanted, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new CheckMedicineStockRequest
                {
                    MedicineId = medicineId,
                    NumberOfMedicineWanted = numberOfMedicineWanted
                };

                _logger.LogInformation("Requesting stock check for MedicineId: {MedicineId}, NumberOfMedicineWanted: {NumberOfMedicineWanted}, RequestId: {RequestId}",
                    medicineId, numberOfMedicineWanted, request.RequestId);

                var response = await _checkMedicineStockRequestClient.GetResponse<CheckMedicineStockResponse>(request, cancellationToken);

                if (response.Message.IsSuccess)
                {
                    _logger.LogInformation("Successfully checked stock for MedicineId: {MedicineId}, IsEnough: {IsEnough}, Difference: {Difference}, RequestId: {RequestId}",
                        medicineId, response.Message.IsEnough, response.Message.Difference, request.RequestId);
                }
                else
                {
                    _logger.LogWarning("Failed to check stock for MedicineId: {MedicineId}, Error: {Error}, RequestId: {RequestId}",
                        medicineId, response.Message.ErrorMessage, request.RequestId);
                }

                return response.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock for MedicineId: {MedicineId}, NumberOfMedicineWanted: {NumberOfMedicineWanted}", medicineId, numberOfMedicineWanted);
                throw;
            }

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

        public async Task<SubtractMedicineBatchStockResponse> SubtractMedicineBatchStockResponseAsync(int medicineBatchId, int quantity, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new SubtractMedicineBatchStockRequest
                {
                    MedicineBatchId = medicineBatchId,
                    Quantity = quantity
                };
                _logger.LogInformation("Requesting to subtract stock for MedicineBatchId: {MedicineBatchId}, Quantity: {Quantity}, RequestId: {RequestId}",
                    medicineBatchId, quantity, request.RequestId);
                var response = await _subtractMedicineBatchStockRequestClient.GetResponse<SubtractMedicineBatchStockResponse>(request, cancellationToken);

                if (response.Message.IsSuccess)
                {
                    _logger.LogInformation("Successfully subtracted stock for MedicineBatchId: " +
                        "{MedicineBatchId}, Quantity: {Quantity}, RequestId: {RequestId}", medicineBatchId, quantity, request.RequestId);
                }
                else
                {
                    _logger.LogWarning("Failed to subtract stock for MedicineBatchId: {MedicineBatchId}, Error: {Error}, RequestId: {RequestId}",
                        medicineBatchId, response.Message.ErrorMessage, request.RequestId);
                }
                return response.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting to subtract stock for MedicineBatchId: {MedicineBatchId}, Quantity: {Quantity}", medicineBatchId, quantity);
                throw;
            }
        }
    }
}
