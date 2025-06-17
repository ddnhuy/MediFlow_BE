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
    public record GetDiseaseGroupsQuery(
            PaginationRequest PaginationRequest,
            string? SearchTerm
        ) : IQuery<GetDiseaseGroupsResult>;

    public record GetDiseaseGroupsResult(PaginatedResult<DiseaseGroupDTO> DiseaseGroups);

    public class GetDiseaseGroupsQueryHandler : IQueryHandler<GetDiseaseGroupsQuery, GetDiseaseGroupsResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly ILogger<GetDiseaseGroupsQuery> _logger;

        public GetDiseaseGroupsQueryHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            ILogger<GetDiseaseGroupsQuery> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _logger = logger;
        }

        public async Task<GetDiseaseGroupsResult> Handle(GetDiseaseGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var (items, totalCount) = await _diseaseGroupRepository.GetPaginatedAsync(
                    request.PaginationRequest.PageIndex,
                    request.PaginationRequest.PageSize,
                    request.SearchTerm,
                    cancellationToken);

                var diseaseGroups = items.Select(dg => new DiseaseGroupDTO(
                    dg.Id,
                    dg.GroupName,
                    dg.Description
                )).ToList();

                var result = new PaginatedResult<DiseaseGroupDTO>(
                    pageIndex: request.PaginationRequest.PageIndex,
                    pageSize: request.PaginationRequest.PageSize,
                    totalItems: totalCount,
                    data: diseaseGroups
                );

                return new GetDiseaseGroupsResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated disease groups");
                throw;
            }
        }
    }
}
