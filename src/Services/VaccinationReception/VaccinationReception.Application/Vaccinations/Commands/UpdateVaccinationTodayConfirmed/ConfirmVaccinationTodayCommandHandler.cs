using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Models;

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
                .Include(r => r.IncomingTransferredVaccinations)
                .FirstOrDefaultAsync(r => r.Id == request.ReceptionId, cancellationToken);

            if (reception == null)
                throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);

            // Filter ReceptionVaccinations to only those with ScheduleDate as today
            var today = DateTime.UtcNow.Date;
            var todayReceptionVaccinations = reception.ReceptionVaccinations
                .Where(rv => rv.ScheduledDate.HasValue && rv.ScheduledDate.Value.Date == today)
                .ToList();

            var todayIncomingTransferredVaccinations = reception.IncomingTransferredVaccinations
                .Where(rv => rv.ScheduledDate.HasValue && rv.ScheduledDate.Value.Date == today)
                .ToList();

            // Combine both collections
            var allTodayVaccinations = todayReceptionVaccinations
                .Concat(todayIncomingTransferredVaccinations)
                .ToList();

            if (!allTodayVaccinations.Any())
                throw new BadRequestException(ExceptionKey.NO_VACCINATION_TODAY_CONFIRMED);

            var rvIds = allTodayVaccinations.Select(rv => rv.Id).ToList();

            // Get all doses for reception vaccinations scheduled today
            var vaccinations = await _context.Vaccinations
                .Include(v => v.ReceptionVaccination)
                .Where(v => rvIds.Contains(v.ReceptionVaccinationId))
                .ToListAsync(cancellationToken);

            foreach (var rv in allTodayVaccinations)
            {
                var related = vaccinations.Where(v => v.ReceptionVaccinationId == rv.Id).ToList();

                if (related.Count < rv.Quantity)
                    throw new BadRequestException(ExceptionKey.ANY_VACCINATION_NOT_CONFIRMED);

                if (related.Any(v => !v.IsConfirmed))
                    throw new BadRequestException(ExceptionKey.ANY_VACCINATION_NOT_CONFIRMED);

                if (related.Any(v => !v.ObservationConfirmed))
                    throw new BadRequestException(ExceptionKey.ANY_POST_VACCINATION_NOT_CONFIRMED);
            }

            reception.IsVaccinationTodayConfirmed = true;
            reception.LastUpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return new ConfirmVaccinationTodayResult(true);
        }
    }
}
