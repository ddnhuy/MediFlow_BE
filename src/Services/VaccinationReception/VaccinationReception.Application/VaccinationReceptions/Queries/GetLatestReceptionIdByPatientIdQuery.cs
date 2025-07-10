using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public record GetLatestReceptionIdByPatientIdQuery(int PatientId) : IQuery<int?>;
    public class GetLatestReceptionIdByPatientIdQueryHandler : IQueryHandler<GetLatestReceptionIdByPatientIdQuery, int?>
    {
        private readonly IApplicationDbContext _context;

        public GetLatestReceptionIdByPatientIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int?> Handle(GetLatestReceptionIdByPatientIdQuery request, CancellationToken cancellationToken)
        {
            var latestReceptionId = await _context.Receptions
                .Where(r => r.PatientId == request.PatientId && !r.IsCancelled)
                .OrderByDescending(r => r.ReceptionDate)
                .Select(r => (int?)r.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return latestReceptionId;
        }
    }
}
