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
    public record GetAllServicesWithDetailsQuery : IQuery<List<ServiceDetailDTO>>;

    public class GetAllServicesWithDetailsQueryHandler : IQueryHandler<GetAllServicesWithDetailsQuery, List<ServiceDetailDTO>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetAllServicesWithDetailsQueryHandler> _logger;

        public GetAllServicesWithDetailsQueryHandler(
            IServiceRepository serviceRepository,
            ILogger<GetAllServicesWithDetailsQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<List<ServiceDetailDTO>> Handle(GetAllServicesWithDetailsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all services with full details");

            try
            {
                var services = await _serviceRepository.GetAllWithDetailsAsync(cancellationToken);

                var serviceDetailDTOs = services
                .Where(s => !s.IsCancelled)
                .Select(s => new ServiceDetailDTO(
                    s.Id,
                    s.ServiceCode,
                    s.ServiceName,
                    s.UnitPrice,
                    s.DepartmentId,
                    s.CreatedAt,
                    s.LastUpdatedAt,
                    s.ServiceGroupServices
                        .Where(sgs => !sgs.IsCancelled && sgs.ServiceGroup != null)
                        .Select(sgs => new ServiceGroupSummaryDTO(
                            sgs.ServiceGroup.Id,
                            sgs.ServiceGroup.GroupName
                        ))
                        .ToList(),
                    s.DiseaseGroupServices
                        .Where(dgs => !dgs.IsCancelled && dgs.DiseaseGroup != null)
                        .Select(dgs => new DiseaseGroupSummaryDTO(
                            dgs.DiseaseGroup.Id,
                            dgs.DiseaseGroup.GroupName,
                            dgs.DiseaseGroup.Description
                        ))
                        .ToList()
                ))
                .OrderBy(s => s.ServiceCode)
                .ToList();

                _logger.LogInformation("Found {Count} services with full details", serviceDetailDTOs.Count);

                return serviceDetailDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all services with details");
                throw;
            }
        }
    }
}
