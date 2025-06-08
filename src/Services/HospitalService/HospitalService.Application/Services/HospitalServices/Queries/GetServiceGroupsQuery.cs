using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
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
    public record GetServiceGroupsQuery(
        PaginationRequest PaginationRequest,
        string? SearchTerm
    ) : IQuery<GetServiceGroupsResult>;

    public record GetServiceGroupsResult(PaginatedResult<ServiceGroupDTO> ServiceGroups);

    public class GetServiceGroupsQueryHandler : IQueryHandler<GetServiceGroupsQuery, GetServiceGroupsResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetServiceGroupsQuery> _logger;

        public GetServiceGroupsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetServiceGroupsQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetServiceGroupsResult> Handle(GetServiceGroupsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.ServiceGroups.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(sg =>
                        sg.GroupName.ToLower().Contains(request.SearchTerm.ToLower()));
                }

                var totalCount = await query.CountAsync(cancellationToken);
                var items = await query
                    .OrderByDescending(sg => sg.CreatedAt)
                    .Skip((request.PaginationRequest.PageIndex - 1) * request.PaginationRequest.PageSize)
                    .Take(request.PaginationRequest.PageSize)
                    .Select(sg => new ServiceGroupDTO(
                        sg.Id,
                        sg.GroupName
                    ))
                    .ToListAsync(cancellationToken);

                var result = new PaginatedResult<ServiceGroupDTO>(
                    pageIndex: request.PaginationRequest.PageIndex,
                    pageSize: request.PaginationRequest.PageSize,
                    totalItems: totalCount,
                    data: items
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
