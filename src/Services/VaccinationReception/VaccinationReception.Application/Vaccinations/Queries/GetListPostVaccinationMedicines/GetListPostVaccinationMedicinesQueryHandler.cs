using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationMedicines
{
    public class GetListPostVaccinationMedicinesQueryHandler : IQueryHandler<GetListPostVaccinationMedicinesQuery, List<GetListPostVaccinationMedicinesResult>>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetListPostVaccinationMedicinesQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GetListPostVaccinationMedicinesResult>> Handle(GetListPostVaccinationMedicinesQuery request, CancellationToken cancellationToken)
        {

            var vaccinations = await _dbContext.Vaccinations
                .Include(v => v.ReceptionVaccination)
                .Where(v => v.ReceptionVaccination!.ReceptionId == request.ReceptionId && !v.ObservationConfirmed).ToListAsync(cancellationToken);

             var result = vaccinations.Select(v => new GetListPostVaccinationMedicinesResult(
                    v.Id,
                    v.MedicineName ?? string.Empty,
                    v.ReceptionVaccination!.Quantity,
                    v.VaccinationDate ?? DateTime.MinValue,
                    v.ObservationConfirmed,
                    v.ReactionDate
                )).ToList();

            return result;
        }
    }
}
