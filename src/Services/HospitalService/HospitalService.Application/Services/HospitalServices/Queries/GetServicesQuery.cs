using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
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
    public record GetServicesQuery(
        PaginationRequest PaginationRequest,
        string? SearchTerm
    ) : IQuery<GetServicesResult>;
    public record GetServicesResult(PaginatedResult<ServiceDTO> Services);

    public class GetServicesQueryHandler : IQueryHandler<GetServicesQuery, GetServicesResult>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ILogger<GetServicesQueryHandler> _logger;

        public GetServicesQueryHandler(IServiceRepository serviceRepository, ILogger<GetServicesQueryHandler> logger)
        {
            _serviceRepository = serviceRepository;
            _logger = logger;
        }

        public async Task<GetServicesResult> Handle(GetServicesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting services with pagination - PageIndex: {PageIndex}, PageSize: {PageSize}, SearchTerm: {SearchTerm}",
                request.PaginationRequest.PageIndex, request.PaginationRequest.PageSize, request.SearchTerm);

            var allServices = await _serviceRepository.GetAllWithDetailsAsync(cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                allServices = allServices
                    .Where(s => s.ServiceCode.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                s.ServiceName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalItems = allServices.LongCount();

            var pagedServices = allServices
                .Skip((request.PaginationRequest.PageIndex - 1) * request.PaginationRequest.PageSize)
                .Take(request.PaginationRequest.PageSize)
                .ToList();

            var serviceDTOs = pagedServices
                .Select(s => new ServiceDTO(
                    s.Id,
                    s.ServiceCode,
                    s.ServiceName,
                    s.ServiceType,
                    s.UnitPrice,
                    s.DepartmentId,
                    s.ExaminationService,
                    s.ServiceTestParameters
                ))
                .ToList();

            var paginatedResult = new PaginatedResult<ServiceDTO>(
                request.PaginationRequest.PageIndex,
                request.PaginationRequest.PageSize,
                totalItems,
                serviceDTOs
            );

            return new GetServicesResult(paginatedResult);
        }
    }
}