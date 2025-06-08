using BuildingBlocks.CQRS;
using HospitalService.Application.DTOs;
using HospitalService.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetAllServiceGroupsQueryHandler> _logger;

        public GetAllServiceGroupsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetAllServiceGroupsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetAllServiceGroupsResult> Handle(GetAllServiceGroupsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all service groups with search: {SearchTerm}", request.SearchTerm);

            try
            {
                var query = _context.ServiceGroups.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(sg =>
                        sg.GroupName.ToLower().Contains(request.SearchTerm.ToLower()));
                }

                var items = await query
                    .OrderByDescending(sg => sg.CreatedAt)
                    .Select(sg => new ServiceGroupDTO(
                        sg.Id,
                        sg.GroupName
                    ))
                    .ToListAsync(cancellationToken);

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