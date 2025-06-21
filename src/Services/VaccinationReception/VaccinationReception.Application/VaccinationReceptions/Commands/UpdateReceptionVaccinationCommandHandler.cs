using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VaccinationReception.Application.Data;

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
                    .FirstOrDefaultAsync(rv => rv.Id == request.Id && !rv.IsCancelled, cancellationToken);

                if (receptionVaccination == null)
                {
                    _logger.LogWarning("Không tìm thấy ReceptionVaccination với Id: {Id}", request.Id);
                    return new UpdateReceptionVaccinationResult(false);
                }

                receptionVaccination.Quantity = request.Quantity;
                receptionVaccination.IsReadyToUse = request.IsReadyToUse;
                receptionVaccination.ScheduledDate = request.ScheduledDate;
                receptionVaccination.InvoiceDate = request.InvoiceDate;
                receptionVaccination.AppointmentDate = request.AppointmentDate;
                receptionVaccination.PaymentStatus = request.PaymentStatus;
                receptionVaccination.IsConfirmed = request.IsConfirmed;
                receptionVaccination.Note = request.Note;
                receptionVaccination.TestResultEntry = request.TestResultEntry;
                receptionVaccination.DoctorId = request.DoctorId;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Đã cập nhật thành công ReceptionVaccination với Id: {Id}", request.Id);

                return new UpdateReceptionVaccinationResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật ReceptionVaccination với Id: {Id}", request.Id);
                throw;
            }
        }
    }
}