using BuildingBlocks.CQRS;
using BuildingBlocks.Strings.Enums;
using HospitalService.Application.DTOs;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HospitalService.Application.Services.HospitalServices.Queries
{
    public record GetAllExaminationServicesQuery(string? SearchTerm) : IQuery<GetAllExaminationServicesResult>;
    public record GetAllExaminationServicesResult(List<ServiceDTO> Services);

    public class GetAllExaminationServicesQueryHandler : IQueryHandler<GetAllExaminationServicesQuery, GetAllExaminationServicesResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetAllExaminationServicesQueryHandler> _logger;

        public GetAllExaminationServicesQueryHandler(IServiceRepository serviceRepository, ILogger<GetAllExaminationServicesQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<GetAllExaminationServicesResult> Handle(GetAllExaminationServicesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Models.Service> services;
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                services = await _serviceRepository.GetAllAsync(cancellationToken);
            else
                services = await _serviceRepository.GetBySearchTermAsync(request.SearchTerm, cancellationToken);

            // Filter for examination services
            var filtered = services
                .Where(s => s.ServiceType == ServiceType.Test && s.ExaminationService != null);

            var dtos = filtered.Select(s => new ServiceDTO(
                s.Id,
                s.ServiceCode,
                s.ServiceName,
                s.ServiceType,
                s.UnitPrice,
                s.DepartmentId,
                s.ExaminationService,
                s.ServiceTestParameters
            )).ToList();

            return new GetAllExaminationServicesResult(dtos);
        }
    }
}