using BuildingBlocks.Messaging.Contracts.Inventory.MedicineBatchInformation.NearestExpiryMedicineBatch;
using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
namespace VaccinationReception.Application.Abstraction.InventoryMessaging
{
    public interface IInventoryService
    {
        Task<List<GetMedicineInformationResponse>> GetMedicineInformationAsync(IEnumerable<int> medicineIdList, CancellationToken cancellationToken = default);
        Task<GetNearestExpiryMedicineBatchResponse> GetNearestExpiryMedicineBatchAsync(int medicineId, CancellationToken cancellationToken = default);
    }
}
