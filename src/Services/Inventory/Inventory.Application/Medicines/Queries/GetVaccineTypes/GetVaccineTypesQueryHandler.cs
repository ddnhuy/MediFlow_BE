using Inventory.Application.Data;
using Inventory.Application.Medicines.Queries.GetVaccineTypes.Inventory.Application.VaccineTypes.Queries;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.VaccineTypes.Queries
{
    public class GetVaccineTypesQueryHandler : IQueryHandler<GetVaccineTypesQuery, GetVaccineTypesResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetVaccineTypesQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetVaccineTypesResult> Handle(GetVaccineTypesQuery request, CancellationToken cancellationToken)
        {
            var vaccineTypes = await _dbContext.VaccineTypes
                .AsNoTracking()
                .OrderBy(x => x.VaccineTypeName)
                .Where(x => !x.IsCancelled)
                .ToListAsync(cancellationToken);

            var dtos = vaccineTypes.Select(v => new VaccineTypeDTO
            (
                VaccineTypeId : v.Id,
                VaccinatTypeName : v.VaccineTypeName ?? ""
            )).ToList();

            return new GetVaccineTypesResult(dtos);
        }
    }
}