using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInteraction;
namespace VaccinationReception.Application.Abstraction.InventoryMessaging
{
    public interface IInventoryService
    {
        Task<List<GetMedicineInformationResponse>> GetMedicineInformationAsync(IEnumerable<int> medicineIdList, CancellationToken cancellationToken = default);
        Task<GetNearestExpiryMedicineBatchResponse> GetNearestExpiryMedicineBatchAsync(int medicineId, CancellationToken cancellationToken = default);
        Task<GetMedicineInteractionsResponse> GetMedicineInteractionsResponseAsync(int medicineId, CancellationToken cancellationToken = default);
    }
}
