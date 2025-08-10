using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Enums;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class DeleteReceptionVaccinationsCommandHandler : ICommandHandler<DeleteReceptionVaccinationsCommand, DeleteReceptionVaccinationsResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteReceptionVaccinationsCommandHandler> _logger;

        public DeleteReceptionVaccinationsCommandHandler(
            IApplicationDbContext context,
            ILogger<DeleteReceptionVaccinationsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteReceptionVaccinationsResult> Handle(DeleteReceptionVaccinationsCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var reception = await _context.Receptions
                    .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionId && !rv.IsCancelled, cancellationToken);

                if (reception != null)
                {
                    reception.LastUpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync(cancellationToken);
                }

                var receptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv =>
                        request.ReceptionVaccinationIds.Contains(rv.Id) &&
                        (rv.ReceptionId == request.ReceptionId || rv.SecondaryReceptionId == request.ReceptionId) &&
                        rv.PaymentStatus == PaymentStatusForItem.NotPaid &&
                        !rv.IsCancelled)
                    .ToListAsync(cancellationToken);

                if (!receptionVaccinations.Any())
                {
                    _logger.LogWarning("No ReceptionVaccination found to delete with Ids: {Ids} and ReceptionId: {ReceptionId}",
                        string.Join(", ", request.ReceptionVaccinationIds),
                        request.ReceptionId);
                    return new DeleteReceptionVaccinationsResult(false, 0);
                }

                foreach (var vaccination in receptionVaccinations)
                {
                    vaccination.IsCancelled = true;
                }

                var requestNumbers = receptionVaccinations
                   .Where(rv => !string.IsNullOrEmpty(rv.RequestNumber))
                   .Select(rv => rv.RequestNumber)
                   .Distinct()
                   .ToList();

                if (requestNumbers.Any())
                {
                    var relatedServiceDetails = await _context.ServiceRequestDetails
                        .Where(s => requestNumbers.Contains(s.RequestNumber)
                                    && s.ReceptionId == request.ReceptionId
                                    && s.PaymentStatus == PaymentStatusForItem.NotPaid
                                    && !s.IsCancelled)
                        .ToListAsync(cancellationToken);

                    foreach (var detail in relatedServiceDetails)
                    {
                        detail.IsCancelled = true;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully deleted {Count} ReceptionVaccination with Ids: {Ids} for ReceptionId: {ReceptionId}",
                    receptionVaccinations.Count,
                    string.Join(", ", receptionVaccinations.Select(rv => rv.Id)),
                    request.ReceptionId);

                return new DeleteReceptionVaccinationsResult(true, receptionVaccinations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting ReceptionVaccination with Ids: {Ids} and ReceptionId: {ReceptionId}",
                    string.Join(", ", request.ReceptionVaccinationIds),
                    request.ReceptionId);
                throw;
            }
        }
    }
}