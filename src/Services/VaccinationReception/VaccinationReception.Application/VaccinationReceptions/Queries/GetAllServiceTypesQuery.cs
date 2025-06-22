using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetAllServiceTypesQuery : IQuery<List<ServiceTypeDTO>>;

    public class GetAllServiceTypesQueryHandler : IQueryHandler<GetAllServiceTypesQuery, List<ServiceTypeDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllServiceTypesQueryHandler> _logger;

        public GetAllServiceTypesQueryHandler(
            IApplicationDbContext context,
            ILogger<GetAllServiceTypesQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ServiceTypeDTO>> Handle(GetAllServiceTypesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting all service types");

            var serviceTypes = await _context.ServiceTypes
                .Where(st => !st.IsCancelled)
                .OrderBy(st => st.Code)
                .Select(st => new ServiceTypeDTO(
                    st.Id,
                    st.Code,
                    st.Name,
                    st.CreatedAt,
                    st.LastUpdatedAt
                ))
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {Count} service types", serviceTypes.Count);

            return serviceTypes;
        }
    }
}
