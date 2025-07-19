using BuildingBlocks.Messaging.Contracts.HospitalService;

namespace VaccinationReception.Application.Abstractions.HospitalServiceMessaging
{
    public interface IHospitalService
    {
        Task<List<ServiceDTO>> GetServicesByGroupAsync(int groupId, string groupType, CancellationToken cancellationToken);
        Task<List<ServiceDTO>> GetServicesByIdsAsync(List<int> serviceIds, CancellationToken cancellationToken);
        Task<List<ServiceDTO>> GetServicesByServiceCodeAsync(List<string> serviceCodes, CancellationToken cancellationToken);
    }
}
