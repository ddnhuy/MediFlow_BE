using BuildingBlocks.CQRS;
using BuildingBlocks.Strings.Consts.HospitalServices;
using BuildingBlocks.Strings.Enums;
using HospitalService.Application.DTOs;
using HospitalService.Domain;
using HospitalService.Domain.Models;
using HospitalService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalService.Application.Services.HospitalServices.Queries
{
    public record GetAllServicesWithoutPaginationQuery(
            string? SearchTerm,
            ServiceType? ServiceType
        ) : IQuery<GetAllServicesWithoutPaginationResult>;

    public record GetAllServicesWithoutPaginationResult(List<ServiceDTO> Services);

    public class GetAllServicesWithoutPaginationQueryHandler : IQueryHandler<GetAllServicesWithoutPaginationQuery, GetAllServicesWithoutPaginationResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetAllServicesWithoutPaginationQueryHandler> _logger;

        public GetAllServicesWithoutPaginationQueryHandler(
            IServiceRepository serviceRepository,
            ILogger<GetAllServicesWithoutPaginationQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<GetAllServicesWithoutPaginationResult> Handle(GetAllServicesWithoutPaginationQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all services with search: {SearchTerm}", request.SearchTerm);

            try
            {
                IEnumerable<Service> services;

                if (string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    services = await _serviceRepository.GetAllAsync(cancellationToken);
                }
                else
                {
                    services = await _serviceRepository.GetBySearchTermAsync(request.SearchTerm, cancellationToken);
                }

                var filteredServices = services;

                if (request.ServiceType != null)
                {
                    filteredServices = filteredServices
                        .Where(s => s.ServiceType == request.ServiceType);
                }

                var items = filteredServices.Select(s => new ServiceDTO(
                    s.Id,
                    s.ServiceCode,
                    s.ServiceName,
                    s.ServiceType,
                    s.UnitPrice,
                    s.DepartmentId,
                    s.ExaminationService,
                    s.ServiceTestParameters
                )).ToList();

                _logger.LogInformation("Found {Count} services", items.Count);
                return new GetAllServicesWithoutPaginationResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all services");
                throw;
            }
        }
    }
}