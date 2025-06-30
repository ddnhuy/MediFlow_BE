using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
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

            var doctorPrescribedVaccines = receptionVaccinations
                .Where(rv => rv.ScheduledDate.Date == today)
                .ToList();

            var customerWarehouseVaccines = receptionVaccinations
                .Where(rv => rv.ScheduledDate.Date > today)
                .ToList();

            // Get unique vaccine IDs for inventory service call
            var allVaccineIds = receptionVaccinations
                .Select(rv => rv.VaccineId)
                .Distinct()
                .ToList();

            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(allVaccineIds, cancellationToken);

            // Create medicine info lookup
            var medicineLookup = medicineInformationList.ToDictionary(m => m.MedicineId, m => m);

            // Map to result types for doctor prescribed vaccines
            var doctorPrescribedTasks = doctorPrescribedVaccines
                .Where(rv => medicineLookup.ContainsKey(rv.VaccineId))
                .Select(async rv => new MedicineInfo(
                    rv.Id,
                    medicineLookup[rv.VaccineId].MedicineId,
                    medicineLookup[rv.VaccineId].MedicineName ?? string.Empty,
                    rv.IsConfirmed,
                    rv.TestResultEntry,
                    await GetDoctorName(rv.DoctorId.Value)
                ))
                .ToList();

            var doctorPrescribedResult = await Task.WhenAll(doctorPrescribedTasks);

            // Map to result types for customer warehouse vaccines
            var customerWarehouseTasks = customerWarehouseVaccines
                .Where(rv => medicineLookup.ContainsKey(rv.VaccineId))
                .Select(async rv => new MedicineInfo(
                    rv.Id,
                    medicineLookup[rv.VaccineId].MedicineId,
                    medicineLookup[rv.VaccineId].MedicineName ?? string.Empty,
                    rv.IsConfirmed,
                    rv.TestResultEntry,
                   await GetDoctorName(rv.DoctorId.Value) 
                ))
                .ToList();

            var customerWarehouseResult = await Task.WhenAll(customerWarehouseTasks);

            return new GetMedicineListForVaccinationByReceptionIdResult(
                doctorPrescribedResult.ToList(),
                customerWarehouseResult.ToList()
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
                Id = id
            }, metadata);

            return $"B.S {doctor.Name}"; 
        }
    }
}
