using BuildingBlocks.Messaging.Contracts.HospitalService;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByGroup;
using BuildingBlocks.Messaging.Contracts.HospitalService.GetServicesByIds;
using MassTransit;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstractions.HospitalServiceMessaging;

namespace VaccinationReception.Infrastructure.Services.HospitalServiceMessaging
{
    public class HospitalService : IHospitalService
    {
        private readonly IRequestClient<GetServicesByGroupRequest> _groupRequestClient;
        private readonly IRequestClient<GetServicesByIdsRequest> _idsRequestClient;
        private readonly ILogger<HospitalService> _logger;

        public HospitalService(
            IRequestClient<GetServicesByGroupRequest> groupRequestClient,
            IRequestClient<GetServicesByIdsRequest> idsRequestClient,
            ILogger<HospitalService> logger)
        {
            _groupRequestClient = groupRequestClient;
            _idsRequestClient = idsRequestClient;
            _logger = logger;
        }

        public async Task<List<ServiceDTO>> GetServicesByGroupAsync(int groupId, string groupType, CancellationToken cancellationToken)
        {
            var request = new GetServicesByGroupRequest { GroupId = groupId, GroupType = groupType };
            var response = await _groupRequestClient.GetResponse<GetServicesByGroupResponse>(request, cancellationToken);
            return response.Message.Services ?? new List<ServiceDTO>();
        }

        public async Task<List<ServiceDTO>> GetServicesByIdsAsync(List<int> serviceIds, CancellationToken cancellationToken)
        {
            var request = new GetServicesByIdsRequest { ServiceIds = serviceIds };
            var response = await _idsRequestClient.GetResponse<GetServicesByIdsResponse>(request, cancellationToken);
            return response.Message.Services ?? new List<ServiceDTO>();
        }
    }
}
