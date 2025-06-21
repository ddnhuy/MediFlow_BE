using BuildingBlocks.CQRS;
using HospitalService.Application.DTOs;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Queries
{
    public record GetServicesByIdsQuery(List<int> ServiceIds) : IQuery<List<ServiceDTO>>;

    public class GetServicesByIdsQueryHandler : IQueryHandler<GetServicesByIdsQuery, List<ServiceDTO>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServicesByIdsQueryHandler> _logger;

        public GetServicesByIdsQueryHandler(
            IServiceRepository serviceRepository,
            ILogger<GetServicesByIdsQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<List<ServiceDTO>> Handle(GetServicesByIdsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting services by IDs: {ServiceIds}", string.Join(", ", request.ServiceIds));

            try
            {
                if (request.ServiceIds == null || !request.ServiceIds.Any())
                {
                    _logger.LogWarning("No service IDs provided");
                    return new List<ServiceDTO>();
                }

                var services = await _serviceRepository.GetByIdsAsync(request.ServiceIds, cancellationToken);

                var serviceDTOs = services.Select(s => new ServiceDTO(
                    s.Id,
                    s.ServiceCode,
                    s.ServiceName,
                    s.UnitPrice,
                    s.DepartmentId
                )).ToList();

                _logger.LogInformation("Found {Count} services out of {RequestedCount} requested",
                    serviceDTOs.Count, request.ServiceIds.Count);

                return serviceDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting services by IDs");
                throw;
            }
        }
    }
}
