using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Vaccinations.Commands.RejectVaccination;

namespace VaccinationReception.Application.VaccinationReceptions.Commands.RejectVaccination
{
    public class RejectVaccinationCommandHandler : ICommandHandler<RejectVaccinationCommand, RejectVaccinationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<RejectVaccinationCommandHandler> _logger;

        public RejectVaccinationCommandHandler(
            IApplicationDbContext context,
            ILogger<RejectVaccinationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RejectVaccinationResult> Handle(RejectVaccinationCommand request, CancellationToken cancellationToken)
        {
            var receptionVaccination = await _context.ReceptionVaccinations
                .Include(rv => rv.Reception)
                .Include(rv => rv.SecondaryReception)
                .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionVaccinationId && !rv.IsCancelled, cancellationToken);

            if (receptionVaccination == null)
                throw new BadRequestException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);

            var existingVaccinations = await _context.Vaccinations
                .Where(v => v.ReceptionVaccinationId == request.ReceptionVaccinationId && v.IsConfirmed)
                .CountAsync(cancellationToken);

            if (existingVaccinations > 0)
            {
                throw new BadRequestException(ExceptionKey.THIS_VACCINE_HAS_BEEN_TAKEN);
            }

            receptionVaccination.HasIssue = true;
            receptionVaccination.IssueNote = request.IssueNote.Trim();
            receptionVaccination.IssueDate = DateTime.UtcNow;
            receptionVaccination.LastUpdatedAt = DateTime.UtcNow;

            var currentReception = receptionVaccination.SecondaryReception ?? receptionVaccination.Reception;
            currentReception.LastUpdatedAt = DateTime.UtcNow;

            if (currentReception.HasIssue == false)
            {
                currentReception.HasIssue = true;
                currentReception.IssueNote = request.IssueNote;
                currentReception.IssueDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("ReceptionVaccination {Id} rejected. Reason: {Reason}",
                request.ReceptionVaccinationId, request.IssueNote);

            return new RejectVaccinationResult(true);
        }
    }
}