using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.Vaccinations.Commands.UpdateVaccinationTodayConfirmed
{
    public class ConfirmVaccinationTodayCommandHandler : ICommandHandler<ConfirmVaccinationTodayCommand, ConfirmVaccinationTodayResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ConfirmVaccinationTodayCommandHandler> _logger;

        public ConfirmVaccinationTodayCommandHandler(IApplicationDbContext context, ILogger<ConfirmVaccinationTodayCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ConfirmVaccinationTodayResult> Handle(ConfirmVaccinationTodayCommand request, CancellationToken cancellationToken)
        {
            var reception = await _context.Receptions
                .Include(r => r.ReceptionVaccinations)
                .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

            if (reception == null)
                throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);

            var rvIds = reception.ReceptionVaccinations.Select(rv => rv.Id).ToList();

            if (reception.ReceptionVaccinations.Any(rv => !rv.IsConfirmed))
                throw new BadRequestException(ExceptionKey.ANY_VACCINATION_NOT_CONFIRMED);

            var vaccinations = await _context.Vaccinations
                .Where(v => rvIds.Contains(v.ReceptionVaccinationId))
                .ToListAsync(cancellationToken);
           
            foreach (var rvId in rvIds)
            {
                var related = vaccinations.Where(v => v.ReceptionVaccinationId == rvId).ToList();
                if (related.Any(v => !v.ObservationConfirmed))
                    throw new BadRequestException(ExceptionKey.ANY_POST_VACCINATION_NOT_CONFIRMED);
            }

            reception.IsVaccinationTodayConfirmed = true;
            await _context.SaveChangesAsync(cancellationToken);

            return new ConfirmVaccinationTodayResult(true);
        }
    }
}
