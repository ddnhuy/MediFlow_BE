using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Vaccinations.Queries.GetPendingVaccinationsTodayQuery;

namespace VaccinationReception.Application.VaccinationReceptions.Queries.GetPendingVaccinationsToday
{
    public class GetPendingVaccinationsTodayQueryHandler : IQueryHandler<GetPendingVaccinationsTodayQuery, GetPendingVaccinationsTodayResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<GetPendingVaccinationsTodayQueryHandler> _logger;

        public GetPendingVaccinationsTodayQueryHandler(
            IApplicationDbContext context,
            IInventoryService inventoryService,
            ILogger<GetPendingVaccinationsTodayQueryHandler> logger)
        {
            _context = context;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task<GetPendingVaccinationsTodayResult> Handle(GetPendingVaccinationsTodayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Kiểm tra Reception tồn tại
                var receptionExists = await _context.Receptions
                    .AnyAsync(r => r.Id == request.ReceptionId && !r.IsCancelled, cancellationToken);

                if (!receptionExists)
                    throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);

                var today = DateTime.UtcNow.Date;

                // Lấy tất cả ReceptionVaccinations được lên lịch cho hôm nay
                var todayReceptionVaccinations = await _context.ReceptionVaccinations
                    .Where(rv =>
                        (rv.ReceptionId == request.ReceptionId || rv.SecondaryReceptionId == request.ReceptionId) &&
                        rv.ScheduledDate.HasValue &&
                        rv.ScheduledDate.Value.Date == today &&
                        !rv.HasIssue &&
                        !rv.IsCancelled)
                    .ToListAsync(cancellationToken);

                if (!todayReceptionVaccinations.Any())
                {
                    _logger.LogInformation("No vaccinations scheduled for today for ReceptionId {ReceptionId}", request.ReceptionId);
                    return new GetPendingVaccinationsTodayResult(0, new List<PendingVaccinationDto>());
                }

                // Lấy tất cả vaccinations đã được thực hiện
                var receptionVaccinationIds = todayReceptionVaccinations.Select(rv => rv.Id).ToList();
                var completedVaccinations = await _context.Vaccinations
                    .Where(v => receptionVaccinationIds.Contains(v.ReceptionVaccinationId) && v.IsConfirmed)
                    .ToListAsync(cancellationToken);

                // Lấy thông tin vaccine từ Inventory service
                var vaccineIds = todayReceptionVaccinations.Select(rv => rv.VaccineId).Distinct().ToList();
                var medicineInfos = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);

                var pendingVaccinations = new List<PendingVaccinationDto>();
                int totalPendingDoses = 0;

                foreach (var receptionVaccination in todayReceptionVaccinations)
                {
                    // Đếm số mũi đã tiêm
                    var completedDoses = completedVaccinations
                        .Where(v => v.ReceptionVaccinationId == receptionVaccination.Id)
                        .Count();

                    // Tính số mũi còn lại
                    var pendingDoses = receptionVaccination.Quantity - completedDoses;

                    if (pendingDoses > 0)
                    {
                        totalPendingDoses += pendingDoses;

                        // Lấy tên vaccine
                        var medicineInfo = medicineInfos.FirstOrDefault(m => m.MedicineId == receptionVaccination.VaccineId);
                        var vaccineName = medicineInfo?.MedicineName ?? $"Vaccine ID {receptionVaccination.VaccineId}";

                        pendingVaccinations.Add(new PendingVaccinationDto(
                            ReceptionVaccinationId: receptionVaccination.Id,
                            VaccineId: receptionVaccination.VaccineId,
                            VaccineName: vaccineName,
                            TotalQuantity: receptionVaccination.Quantity,
                            CompletedDoses: completedDoses,
                            PendingDoses: pendingDoses,
                            ScheduledDate: receptionVaccination.ScheduledDate
                        ));
                    }
                }

                _logger.LogInformation("Found {TotalPending} pending doses from {VaccineCount} vaccines for ReceptionId {ReceptionId}",
                    totalPendingDoses, pendingVaccinations.Count, request.ReceptionId);

                return new GetPendingVaccinationsTodayResult(totalPendingDoses, pendingVaccinations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving pending vaccinations for ReceptionId {ReceptionId}",
                    request.ReceptionId);
                throw;
            }
        }
    }
}