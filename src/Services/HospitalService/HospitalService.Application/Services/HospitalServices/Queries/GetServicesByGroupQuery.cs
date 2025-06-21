using BuildingBlocks.CQRS;
using HospitalService.Domain.Repositories;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Queries
{
    public record GetServicesByGroupQuery(int GroupId, string GroupType) : IQuery<List<GetServicesByGroupResponse>>;
    public record GetServicesByGroupResponse(
        int Id,
        string ServiceCode,
        string ServiceName,
        decimal UnitPrice,
        int DepartmentId
    );

    public class GetServicesByGroupQueryHandler : IQueryHandler<GetServicesByGroupQuery, List<GetServicesByGroupResponse>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServicesByGroupQueryHandler> _logger;


        public GetServicesByGroupQueryHandler(IServiceRepository serviceRepository, ILogger<GetServicesByGroupQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<List<GetServicesByGroupResponse>> Handle(GetServicesByGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var services = await _serviceRepository.GetServicesByGroupIdAsync(
                    request.GroupId,
                    request.GroupType,
                    cancellationToken);
                return services.Adapt<List<GetServicesByGroupResponse>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting services by group");
                throw;
            }
        }
    }
}
