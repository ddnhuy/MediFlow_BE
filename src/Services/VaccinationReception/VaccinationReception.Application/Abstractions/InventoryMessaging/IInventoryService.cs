using BuildingBlocks.Messaging.Contracts.Inventory.MedicineInformation;
namespace VaccinationReception.Application.Abstraction.InventoryMessaging
{
    public interface IInventoryService
    {
        Task<List<GetMedicineInformationResponse>> GetMedicineInformationAsync(IEnumerable<int> medicineIdList, CancellationToken cancellationToken = default);
    }
}
