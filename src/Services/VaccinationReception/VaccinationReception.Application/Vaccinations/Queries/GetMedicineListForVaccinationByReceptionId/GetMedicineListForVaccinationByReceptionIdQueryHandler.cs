using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VaccinationReception.Application.Vaccinations.Queries.GetMedicineListForVaccinationByReceptionId
{
    public class GetMedicineListForVaccinationByReceptionIdQueryHandler : IQueryHandler<GetMedicineListForVaccinationByReceptionIdQuery, GetMedicineListForVaccinationByReceptionIdResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IInventoryService _inventoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;

        public GetMedicineListForVaccinationByReceptionIdQueryHandler(IApplicationDbContext dbContext,
            IInventoryService inventoryService,
            IHttpContextAccessor httpContextAccessor,
            ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto)
        {
            _dbContext = dbContext;
            _inventoryService = inventoryService;
            _httpContextAccessor = httpContextAccessor;
            _applicationUserProto = applicationUserProto;
        }

        public async Task<GetMedicineListForVaccinationByReceptionIdResult> Handle(GetMedicineListForVaccinationByReceptionIdQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            // Get all reception vaccinations for the given reception ID
            var receptionVaccinations = _dbContext.ReceptionVaccinations
                .Where(rv => rv.ReceptionId == request.ReceptionId && rv.IsReadyToUse == true)
                .ToList();

            var receptionVaccinationIds = receptionVaccinations.Select(rv => rv.Id).ToList();

            // Query all Vaccinations for these ReceptionVaccinationIds
            var vaccinations = _dbContext.Vaccinations
                .Where(v => receptionVaccinationIds.Contains(v.ReceptionVaccinationId))
                .ToList();

            // Get unique vaccine IDs for inventory service call
            var allVaccineIds = receptionVaccinations
                .Select(rv => rv.VaccineId)
                .Distinct()
                .ToList();

            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(allVaccineIds, cancellationToken);

            // Create medicine info lookup
            var medicineLookup = medicineInformationList.ToDictionary(m => m.MedicineId, m => m);

            // Map to result types for doctor prescribed vaccines (ScheduledDate == today)
            var doctorPrescribedVaccines = receptionVaccinations
                .Where(rv => rv.ScheduledDate?.Date == today)
                .ToList();

            var doctorPrescribedResult = new List<MedicineInfo>();
            foreach (var rv in doctorPrescribedVaccines)
            {
                if (!medicineLookup.ContainsKey(rv.VaccineId)) continue;
                var doses = vaccinations.Where(v => v.ReceptionVaccinationId == rv.Id).ToList();

                for (int i = 1; i <= rv.Quantity; i++)
                {
                    var dose = doses.FirstOrDefault(d => d.DoseNumber == i);
                    doctorPrescribedResult.Add(new MedicineInfo(
                        ReceptionVaccinationId: rv.Id,
                        VaccinationId: dose?.Id, // Nullable in case it's not yet created
                        MedicineId: medicineLookup[rv.VaccineId].MedicineId,
                        MedicineName: medicineLookup[rv.VaccineId].MedicineName ?? string.Empty,
                        MedicineBatchId: dose?.MedicineBatchId ?? 0,
                        MedicineBatchNumber: dose?.BatchNumber ?? "",
                        IsConfirmed: dose?.IsConfirmed ?? false,
                        IsRequiredTesting: medicineLookup[rv.VaccineId].IsRequiredTestingBeforeUse ?? false,
                        TestResultEntry: rv.TestResultEntry,
                        doctorName: rv.DoctorId.HasValue ? await GetDoctorName(rv.DoctorId.Value) : ""
                    ));
                }
            }

            // Map to result types for customer warehouse vaccines (ScheduledDate > today)
            var customerWarehouseVaccines = receptionVaccinations
                .Where(rv => rv.ScheduledDate?.Date > today)
                .ToList();

            var customerWarehouseResult = new List<MedicineInfo>();
            foreach (var rv in customerWarehouseVaccines)
            {
                if (!medicineLookup.ContainsKey(rv.VaccineId)) continue;
                var doses = vaccinations.Where(v => v.ReceptionVaccinationId == rv.Id).ToList();

                for (int i = 1; i <= rv.Quantity; i++)
                {
                    var dose = doses.FirstOrDefault(d => d.DoseNumber == i);
                    customerWarehouseResult.Add(new MedicineInfo(
                        rv.Id,
                        dose?.Id,
                        medicineLookup[rv.VaccineId].MedicineId,
                        medicineLookup[rv.VaccineId].MedicineName ?? string.Empty,
                        dose?.MedicineBatchId ?? 0,
                        dose?.BatchNumber ?? "",
                        dose?.IsConfirmed ?? false,
                        IsRequiredTesting: medicineLookup[rv.VaccineId].IsRequiredTestingBeforeUse ?? false,
                        rv.TestResultEntry,
                        doctorName: rv.DoctorId.HasValue ? await GetDoctorName(rv.DoctorId.Value) : ""
                    ));
                }
            }

            return new GetMedicineListForVaccinationByReceptionIdResult(
                doctorPrescribedResult,
                customerWarehouseResult
            );
        }

        private async Task<string> GetDoctorName(int doctorId)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int id = int.TryParse(userIdClaim, out var parsedId) ? parsedId : 1;

            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

            var doctor = await _applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest
            {
                Id = doctorId
            }, metadata);

            return $"B.S {doctor.Name}";
        }
    }
}
