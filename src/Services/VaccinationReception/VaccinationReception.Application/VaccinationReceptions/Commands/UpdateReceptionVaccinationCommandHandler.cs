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
    public class UpdateReceptionVaccinationCommandHandler : ICommandHandler<UpdateReceptionVaccinationCommand, UpdateReceptionVaccinationResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateReceptionVaccinationCommandHandler> _logger;

        public UpdateReceptionVaccinationCommandHandler(
            IApplicationDbContext context,
            ILogger<UpdateReceptionVaccinationCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateReceptionVaccinationResult> Handle(UpdateReceptionVaccinationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var receptionVaccination = await _context.ReceptionVaccinations
                    .FirstOrDefaultAsync(rv =>
                        rv.Id == request.Id &&
                        rv.ReceptionId == request.ReceptionId &&
                        rv.PaymentStatus == PaymentStatusForItem.NotPaid
                        && !rv.IsCancelled, cancellationToken);

                if (receptionVaccination == null)
                {
                    _logger.LogWarning("ReceptionVaccination with Id: {Id} not found in ReceptionId: {ReceptionId}", request.Id, request.ReceptionId);
                    return new UpdateReceptionVaccinationResult(false);
                }

                receptionVaccination.Quantity = request.Quantity;
                receptionVaccination.IsReadyToUse = request.IsReadyToUse;
                receptionVaccination.ScheduledDate = request.ScheduledDate;
                receptionVaccination.AppointmentDate = request.AppointmentDate;
                receptionVaccination.Note = request.Note;

                if (!string.IsNullOrEmpty(receptionVaccination.RequestNumber))
                {
                    var relatedServiceRequestDetail = await _context.ServiceRequestDetails
                        .FirstOrDefaultAsync(s =>
                            s.RequestNumber == receptionVaccination.RequestNumber &&
                            s.PaymentStatus == PaymentStatusForItem.NotPaid &&
                            s.ReceptionId == request.ReceptionId, cancellationToken);

                    if (relatedServiceRequestDetail != null)
                    {
                        relatedServiceRequestDetail.Quantity = request.Quantity;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully updated ReceptionVaccination with Id: {Id}", request.Id);
                return new UpdateReceptionVaccinationResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating ReceptionVaccination with Id: {Id}", request.Id);
                throw;
            }
        }
    }
}