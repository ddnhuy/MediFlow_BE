using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Vaccinations.Queries.GetAllVaccinationHistory
{
    public class GetAllVaccinationHistoryQueryHandler : IQueryHandler<GetAllVaccinationHistoryQuery, PaginatedResult<AllVaccinationHistoryItem>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;

        public GetAllVaccinationHistoryQueryHandler(
            IApplicationDbContext dbContext,
            IPatientGrpcClient patientGrpcClient,
            IInventoryService inventoryService,
            ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
            _applicationUserProto = applicationUserProto;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedResult<AllVaccinationHistoryItem>> Handle(GetAllVaccinationHistoryQuery request, CancellationToken cancellationToken)
        {
            // Set default date range if not provided (last 30 days)
            var fromDate = request.FromDate?.Date ?? DateTime.UtcNow.AddDays(-30).Date;
            var toDate = request.ToDate?.Date ?? DateTime.UtcNow.Date;

            // Convert to UTC if they're not already to avoid PostgreSQL issues
            if (fromDate.Kind != DateTimeKind.Utc)
                fromDate = DateTime.SpecifyKind(fromDate, DateTimeKind.Utc);

            if (toDate.Kind != DateTimeKind.Utc)
                toDate = DateTime.SpecifyKind(toDate.AddDays(1), DateTimeKind.Utc); // Include end of day
            else
                toDate = toDate.AddDays(1); // Include end of day

            // Case 1a & Case 2: Get ReceptionVaccinations that have actual vaccinations within the date range
            // (Both successful vaccinations and vaccinations with adverse reactions)
            var receptionVaccinationIdsWithVaccinations = await _dbContext.Vaccinations
                .Where(v => v.VaccinationDate.HasValue &&
                           v.VaccinationDate.Value >= fromDate &&
                           v.VaccinationDate.Value < toDate)
                .Select(v => v.ReceptionVaccinationId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Case 1b: Get ReceptionVaccinations that were rejected before injection within the date range
            // Filter by AppointmentDate for vaccines that were never injected
            var receptionVaccinationIdsRejected = await _dbContext.ReceptionVaccinations
                .Where(rv => !_dbContext.Vaccinations.Any(v => v.ReceptionVaccinationId == rv.Id) && // No actual vaccinations
                           rv.HasIssue && // ReceptionVaccination has issue (rejected)
                           rv.ScheduledDate >= fromDate &&
                           rv.ScheduledDate < toDate)
                .Select(rv => rv.Id)
                .ToListAsync(cancellationToken);

            // Combine both sets of ReceptionVaccination IDs
            var allReceptionVaccinationIds = receptionVaccinationIdsWithVaccinations
                .Concat(receptionVaccinationIdsRejected)
                .Distinct()
                .ToList();

            // Get all ReceptionVaccinations that match our criteria
            var allReceptionVaccinations = await _dbContext.ReceptionVaccinations
                .Include(rv => rv.Reception)
                .Include(rv => rv.SecondaryReception)
                .Where(rv => allReceptionVaccinationIds.Contains(rv.Id))
                .ToListAsync(cancellationToken);

            // Get all vaccinations for these ReceptionVaccinations
            var allVaccinations = await _dbContext.Vaccinations
                .Where(v => allReceptionVaccinationIds.Contains(v.ReceptionVaccinationId))
                .ToListAsync(cancellationToken);

            // Get unique patient IDs and medicine IDs
            var patientIds = allReceptionVaccinations.Select(rv => rv.Reception.PatientId).Distinct().ToList();
            var medicineIds = allReceptionVaccinations.Select(rv => rv.VaccineId).Distinct().ToList();

            // Get patient information and medicine information in parallel
            var patientsTask = _patientGrpcClient.ListPatientsByIdsAndSearchAsync(patientIds, null, cancellationToken);
            var medicineInformationTask = _inventoryService.GetMedicineInformationAsync(medicineIds, cancellationToken);

            await Task.WhenAll(patientsTask, medicineInformationTask);

            var patients = await patientsTask;
            var medicineInformationList = await medicineInformationTask;

            var patientDict = patients.ToDictionary(p => p.Id, p => p);
            var medicineInfoDict = medicineInformationList.ToDictionary(m => m.MedicineId, m => m);

            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

            var historyItems = new List<AllVaccinationHistoryItem>();

            foreach (var receptionVaccination in allReceptionVaccinations)
            {
                var patient = patientDict.GetValueOrDefault(receptionVaccination.Reception.PatientId);
                if (patient == null) continue; // Skip if patient not found

                var medicineInfo = medicineInfoDict.GetValueOrDefault(receptionVaccination.VaccineId);
                var currentReceptionId = receptionVaccination.SecondaryReceptionId ?? receptionVaccination.ReceptionId;

                // Get doctor name if available
                string doctorName = string.Empty;
                if (receptionVaccination.DoctorId.HasValue)
                {
                    var doctor = await _applicationUserProto.GetApplicationUserAsync(
                        new GetApplicationUserRequest { Id = receptionVaccination.DoctorId.Value }, metadata);
                    doctorName = doctor.Name ?? string.Empty;
                }

                // Get vaccinations for this ReceptionVaccination
                var vaccinationsForThisRV = allVaccinations
                    .Where(v => v.ReceptionVaccinationId == receptionVaccination.Id)
                    .ToList();

                if (vaccinationsForThisRV.Any())
                {
                    // Has actual vaccinations - check each vaccination individually
                    foreach (var vaccination in vaccinationsForThisRV)
                    {
                        // Check if this specific vaccination has reactions
                        var hasVaccinationReaction = vaccination.HasReaction;

                        historyItems.Add(new AllVaccinationHistoryItem(
                            Id: vaccination.Id,
                            ReceptionId: currentReceptionId,
                            ReceptionVaccinationId: receptionVaccination.Id,
                            PatientId: patient.Id,
                            PatientCode: patient.Code,
                            PatientName: patient.Name,
                            MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                            MedicineName: vaccination.MedicineName ?? string.Empty,
                            DoseNumber: $"Mũi thứ {vaccination.DoseNumber}",
                            VaccinationTestDate: receptionVaccination.VaccinationTestDate,
                            VaccinationDate: vaccination.VaccinationDate,
                            VaccinationConfirmation: vaccination.IsConfirmed,
                            DoctorName: $"B.S {doctorName}",
                            HasIssue: hasVaccinationReaction,
                            IssueNote: hasVaccinationReaction ? receptionVaccination.Reception.IssueNote : null,
                            IssueDate: hasVaccinationReaction ? receptionVaccination.Reception.IssueDate : null
                        ));
                    }
                }
                else if (receptionVaccination.HasIssue)
                {
                    // Case 1b: Vaccine was rejected before injection (receptionVaccination.HasIssue = true)
                    // Create entries based on the planned quantity
                    for (int doseNum = 1; doseNum <= receptionVaccination.Quantity; doseNum++)
                    {
                        historyItems.Add(new AllVaccinationHistoryItem(
                            Id: null,
                            ReceptionId: currentReceptionId,
                            ReceptionVaccinationId: receptionVaccination.Id,
                            PatientId: patient.Id,
                            PatientCode: patient.Code,
                            PatientName: patient.Name,
                            MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                            MedicineName: medicineInfo?.MedicineName ?? string.Empty,
                            DoseNumber: "N/A",
                            VaccinationTestDate: receptionVaccination.VaccinationTestDate,
                            VaccinationDate: null, // No vaccination date since it was rejected
                            VaccinationConfirmation: false,
                            DoctorName: !string.IsNullOrEmpty(doctorName) ? $"B.S {doctorName}" : string.Empty,
                            HasIssue: true,
                            IssueNote: receptionVaccination.IssueNote,
                            IssueDate: receptionVaccination.IssueDate
                        ));
                    }
                }
            }

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim();
                historyItems = historyItems.Where(item =>
                    (item.PatientName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.PatientCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.MedicineName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (item.MedicineTypeName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            // Order by descending vaccination date (as requested)
            var sortedItems = historyItems
                .OrderByDescending(x => x.VaccinationDate ?? x.VaccinationTestDate ?? DateTime.MinValue)
                .ToList();

            // Apply pagination
            var totalCount = sortedItems.Count;
            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var pagedData = sortedItems
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<AllVaccinationHistoryItem>(pageIndex, pageSize, totalCount, pagedData);
        }
    }
}