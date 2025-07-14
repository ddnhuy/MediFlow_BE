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
    public record GetAllServiceGroupsQuery(
        string? SearchTerm
    ) : IQuery<GetAllServiceGroupsResult>;

    public record GetAllServiceGroupsResult(List<ServiceGroupDTO> ServiceGroups);

    public class GetAllServiceGroupsQueryHandler : IQueryHandler<GetAllServiceGroupsQuery, GetAllServiceGroupsResult>
    {
        private readonly IServiceGroupRepository _serviceGroupRepository;
        private readonly ILogger<GetAllServiceGroupsQueryHandler> _logger;

        public GetAllServiceGroupsQueryHandler(
            IServiceGroupRepository serviceGroupRepository,
            ILogger<GetAllServiceGroupsQueryHandler> logger)
        {
            _serviceGroupRepository = serviceGroupRepository;
            _logger = logger;
        }

        public async Task<GetAllServiceGroupsResult> Handle(GetAllServiceGroupsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all service groups with search: {SearchTerm}", request.SearchTerm);

            try
            {
                var serviceGroups = await _serviceGroupRepository.GetAllAsync(request.SearchTerm, cancellationToken);

                var items = serviceGroups.Select(sg => new ServiceGroupDTO(
                    sg.Id,
                    sg.GroupName
                )).ToList();

                _logger.LogInformation("Found {Count} service groups", items.Count);
                return new GetAllServiceGroupsResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all service groups");
                throw;
            }
        }
    }
}