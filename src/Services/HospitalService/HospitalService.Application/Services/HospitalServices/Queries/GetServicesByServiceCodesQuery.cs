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
    public record GetServicesByServiceCodesQuery(List<string> ServiceCodes) : IQuery<List<ServiceDTO>>;
    public class GetServicesByServiceCodesQueryHandler : IQueryHandler<GetServicesByServiceCodesQuery, List<ServiceDTO>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServicesByServiceCodesQueryHandler> _logger;

        public GetServicesByServiceCodesQueryHandler(
            IServiceRepository serviceRepository,
            ILogger<GetServicesByServiceCodesQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<List<ServiceDTO>> Handle(GetServicesByServiceCodesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting services by IDs: {ServiceIds}", string.Join(", ", request.ServiceCodes));

            try
            {
                if (request.ServiceCodes == null || !request.ServiceCodes.Any())
                {
                    _logger.LogWarning("No service IDs provided");
                    return new List<ServiceDTO>();
                }

                var services = await _serviceRepository.GetByServiceCodesAsync(request.ServiceCodes, cancellationToken);

                var serviceDTOs = services.Select(s => new ServiceDTO(
                    s.Id,
                    s.ServiceCode,
                    s.ServiceName,
                    s.UnitPrice,
                    s.DepartmentId
                )).ToList();

                _logger.LogInformation("Found {Count} services out of {RequestedCount} requested",
                    serviceDTOs.Count, request.ServiceCodes.Count);

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
