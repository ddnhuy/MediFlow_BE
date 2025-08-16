using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.VaccinationReceptions.EventHandlers;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class CloseReceptionWithIssueCommandHandler : ICommandHandler<CloseReceptionWithIssueCommand, CloseReceptionWithIssueResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CloseReceptionWithIssueCommandHandler> _logger;
        private readonly IPublisher _publisher;
        private readonly IInventoryService _inventoryService;

        public CloseReceptionWithIssueCommandHandler(
            IApplicationDbContext context,
            ILogger<CloseReceptionWithIssueCommandHandler> logger,
            IPublisher publisher,
            IInventoryService inventoryService)
        {
            _context = context;
            _logger = logger;
            _publisher = publisher;
            _inventoryService = inventoryService;
        }

        public async Task<CloseReceptionWithIssueResult> Handle(CloseReceptionWithIssueCommand request, CancellationToken cancellationToken)
        {
            var reception = await _context.Receptions
                .FirstOrDefaultAsync(r => r.Id == request.ReceptionId && !r.IsCancelled, cancellationToken);

            if (reception == null)
                throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);

            if (reception.IsVaccinationTodayConfirmed && !string.IsNullOrEmpty(reception.IssueNote))
                throw new BadRequestException(ExceptionKey.RECEPTION_HAS_BEEN_CLOSED);

            reception.HasIssue = true;
            reception.IssueNote = request.IssueNote.Trim();
            reception.IssueDate = DateTime.UtcNow;
            reception.IsVaccinationTodayConfirmed = true;
            reception.LastUpdatedAt = DateTime.UtcNow;

            if (request.ReScheduleDate.HasValue)
            {
                int rescheduledCount = await RescheduleRemainingVaccinesToday(
                    request.ReceptionId,
                    request.ReScheduleDate.Value,
                    request.IssueNote,
                    cancellationToken);

                if (rescheduledCount > 0)
                {
                    reception.IssueNote = $"{request.IssueNote.Trim()}. Đã hẹn lại {rescheduledCount} mũi tiêm đến ngày {request.ReScheduleDate.Value:dd/MM/yyyy}.";

                    _logger.LogInformation("Rescheduled {Count} vaccinations to {Date} for Reception {ReceptionId}",
                        rescheduledCount, request.ReScheduleDate.Value, request.ReceptionId);
                }
            }        

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("Reception {ReceptionId} manually closed with issue: {IssueNote}",
                request.ReceptionId, request.IssueNote);

            return new CloseReceptionWithIssueResult(true);
        }

        private async Task<int> RescheduleRemainingVaccinesToday(
            int receptionId,
            DateTime rescheduleDate,
            string rescheduleReason,
            CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            // Lấy tất cả ReceptionVaccinations được lên lịch cho hôm nay
            var todayReceptionVaccinations = await _context.ReceptionVaccinations
                .Include(r => r.Reception)
                .Where(rv =>
                    (rv.ReceptionId == receptionId || rv.SecondaryReceptionId == receptionId) &&
                    rv.ScheduledDate.HasValue &&
                    rv.ScheduledDate.Value.Date == today &&
                    !rv.IsCancelled)
                .ToListAsync(cancellationToken);

            if (!todayReceptionVaccinations.Any())
                return 0;

            // Lấy thông tin các vaccination đã hoàn thành
            var receptionVaccinationIds = todayReceptionVaccinations.Select(rv => rv.Id).ToList();
            var completedVaccinations = await _context.Vaccinations
                .Where(v => receptionVaccinationIds.Contains(v.ReceptionVaccinationId) && v.IsConfirmed)
                .ToListAsync(cancellationToken);

            int rescheduledCount = 0;

            foreach (var receptionVaccination in todayReceptionVaccinations)
            {
                var completedDoses = completedVaccinations
                    .Where(v => v.ReceptionVaccinationId == receptionVaccination.Id)
                    .Count();

                var medicineList = await _inventoryService.GetMedicineInformationAsync([receptionVaccination.VaccineId], cancellationToken); 
                var medicine = medicineList.FirstOrDefault(m => m.MedicineId == receptionVaccination.VaccineId);

                if (completedDoses < receptionVaccination.Quantity)
                {
                    var pendingDoses = receptionVaccination.Quantity - completedDoses;

                    receptionVaccination.ScheduledDate = rescheduleDate;
                    receptionVaccination.AppointmentDate = rescheduleDate;
                    receptionVaccination.HasIssue = true;
                    receptionVaccination.IssueNote = $"{rescheduleReason}";
                    receptionVaccination.IssueDate = DateTime.UtcNow;
                    receptionVaccination.LastUpdatedAt = DateTime.UtcNow;

                    rescheduledCount += pendingDoses;

                    var createdEvent = new ReceptionVaccinationCreatedEvent
                    {
                        PatientId = receptionVaccination.Reception.PatientId,
                        VaccineId = receptionVaccination.VaccineId,
                        AppointmentDate = rescheduleDate,
                        Note = $"{rescheduleReason}",                        
                        VaccineName = medicine!.MedicineName,
                        Dose = "N/A",
                        DoctorId = receptionVaccination.DoctorId.Value
                    };
                    await _publisher.Publish(createdEvent, cancellationToken);

                    _logger.LogInformation("Rescheduled ReceptionVaccination {Id}: {Pending}/{Total} doses from {OldDate} to {NewDate}",
                        receptionVaccination.Id, pendingDoses, receptionVaccination.Quantity, today, rescheduleDate);
                }
            }

            return rescheduledCount;
        }
    }
}
