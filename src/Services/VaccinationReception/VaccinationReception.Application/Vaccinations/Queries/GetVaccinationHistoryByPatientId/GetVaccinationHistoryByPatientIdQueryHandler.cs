using BuildingBlocks.CQRS;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId
{
    public class GetVaccinationHistoryByPatientIdQueryHandler : IQueryHandler<GetVaccinationHistoryByPatientIdQuery, GetVaccinationHistoryByPatientIdResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProto;

        public GetVaccinationHistoryByPatientIdQueryHandler(IApplicationDbContext dbContext, IPatientGrpcClient patientGrpcClient, IInventoryService inventoryService, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProto, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
            _applicationUserProto = applicationUserProto;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetVaccinationHistoryByPatientIdResult> Handle(GetVaccinationHistoryByPatientIdQuery request, CancellationToken cancellationToken)
        {
            var patientInfo = await _patientGrpcClient.GetPatientAsync(request.PatientId, cancellationToken);

            var vaccinationHistory = await _dbContext.Vaccinations
                .Include(v => v.ReceptionVaccination)
                .Where(v => v.PatientId == request.PatientId)
                .ToListAsync(cancellationToken);

            // Get unique medicine IDs to fetch medicine information
            var medicineIds = vaccinationHistory.Select(v => v.MedicineId).Distinct().ToList();
            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(medicineIds, cancellationToken);

            // Create a dictionary for quick lookup
            var medicineInfoDict = medicineInformationList.ToDictionary(m => m.MedicineId, m => m);

            // Asynchronously fetch doctor names and build history items
            var vaccinationHistoryItemsTasks = vaccinationHistory.Select(async v =>
            {
                var medicineInfo = medicineInfoDict.GetValueOrDefault(v.MedicineId);

                var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

                var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

                var doctor = await _applicationUserProto.GetApplicationUserAsync(new GetApplicationUserRequest { Id = v.DoctorId }, metadata);
                var doctorName = doctor.Name ?? string.Empty;

                return new VaccinationHistoryItem(
                    Id: v.Id,
                    MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                    MedicineName: v.MedicineName ?? string.Empty,
                    DoseNumber: $"Mũi thứ {v.DoseNumber}",
                    VaccinationTestDate: v.ReceptionVaccination?.VaccinationTestDate,
                    VaccinationDate: v.VaccinationDate!.Value,
                    VaccinationConfirmation: v.IsConfirmed,
                    DoctorName: $"B.S {doctorName}"
                );
            }).ToList();

            var vaccinationHistoryItems = await Task.WhenAll(vaccinationHistoryItemsTasks);

            var history = new GetVaccinationHistoryByPatientIdResult(
                PatientCode: patientInfo.Code,
                PatientVaccinationCode: "",
                PatientName: patientInfo.Name,
                Gender: patientInfo.Gender == 0 ? "Nữ" : "Nam",
                PhoneNumber: patientInfo.PhoneNumber ?? "",
                AddressDetail: patientInfo.AddressDetail ?? "",
                Ward: patientInfo.Ward ?? "",
                District: patientInfo.District ?? "",
                Province: patientInfo.Province ?? "",
                VaccinationHistoryItems: vaccinationHistoryItems
                    .OrderByDescending(x => x.VaccinationDate)
                    .OrderBy(x => x.MedicineName)
                    .ThenBy(x => x.DoseNumber).ToList()
            );

            return history;
        }
    }
}
