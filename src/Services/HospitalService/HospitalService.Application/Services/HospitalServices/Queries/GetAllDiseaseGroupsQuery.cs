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
    public record GetAllDiseaseGroupsQuery(
            string? SearchTerm
        ) : IQuery<GetAllDiseaseGroupsResult>;

    public record GetAllDiseaseGroupsResult(List<DiseaseGroupDTO> DiseaseGroups);

    public class GetAllDiseaseGroupsQueryHandler : IQueryHandler<GetAllDiseaseGroupsQuery, GetAllDiseaseGroupsResult>
    {
        private readonly IDiseaseGroupRepository _diseaseGroupRepository;
        private readonly ILogger<GetAllDiseaseGroupsQueryHandler> _logger;

        public GetAllDiseaseGroupsQueryHandler(
            IDiseaseGroupRepository diseaseGroupRepository,
            ILogger<GetAllDiseaseGroupsQueryHandler> logger)
        {
            _diseaseGroupRepository = diseaseGroupRepository;
            _logger = logger;
        }

        public async Task<GetAllDiseaseGroupsResult> Handle(GetAllDiseaseGroupsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all disease groups with search: {SearchTerm}", request.SearchTerm);

            try
            {
                var diseaseGroups = await _diseaseGroupRepository.GetAllAsync(request.SearchTerm, cancellationToken);

                var items = diseaseGroups.Select(dg => new DiseaseGroupDTO(
                    dg.Id,
                    dg.GroupName,
                    dg.Description
                )).ToList();

                _logger.LogInformation("Found {Count} disease groups", items.Count);
                return new GetAllDiseaseGroupsResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all disease groups");
                throw;
            }
        }
    }
}
