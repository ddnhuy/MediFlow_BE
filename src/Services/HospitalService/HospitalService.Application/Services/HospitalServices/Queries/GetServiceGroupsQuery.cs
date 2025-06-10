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
    public record GetServiceGroupsQuery(
        PaginationRequest PaginationRequest,
        string? SearchTerm
    ) : IQuery<GetServiceGroupsResult>;

    public record GetServiceGroupsResult(PaginatedResult<ServiceGroupDTO> ServiceGroups);

    public class GetServiceGroupsQueryHandler : IQueryHandler<GetServiceGroupsQuery, GetServiceGroupsResult>
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly ILogger<GetServiceGroupsQuery> _logger;

        public GetServiceGroupsQueryHandler(
            IServiceGroupRepository serviceGroupRepository,
            ILogger<GetServiceGroupsQuery> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _logger = logger;
        }

        public async Task<GetServiceGroupsResult> Handle(GetServiceGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (items, totalCount) = await _serviceGroupRepository.GetPaginatedAsync(
                    request.PaginationRequest.PageIndex,
                    request.PaginationRequest.PageSize,
                    request.SearchTerm,
                    cancellationToken);

                var serviceGroups = items.Select(sg => new ServiceGroupDTO(
                    sg.Id,
                    sg.GroupName
                )).ToList();

                var result = new PaginatedResult<ServiceGroupDTO>(
                    pageIndex: request.PaginationRequest.PageIndex,
                    pageSize: request.PaginationRequest.PageSize,
                    totalItems: totalCount,
                    data: serviceGroups
                );

                return new GetServiceGroupsResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated service groups");
                throw;
            }
        }
    }
}
