using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using HumanResource.Grpc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Helpers;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.VaccinationReceptions.Queries
{
    public class GetMedicineListForPreExaminationQueryHandler : IQueryHandler<GetMedicineListForPreExaminationQuery, GetMedicineListForPreExaminationResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IInventoryService _inventoryService;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProtoServiceClient;

        public GetMedicineListForPreExaminationQueryHandler(IApplicationDbContext dbContext, ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProtoServiceClient, IPatientGrpcClient patientGrpcClient, IInventoryService inventoryService, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _applicationUserProtoServiceClient = applicationUserProtoServiceClient;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetMedicineListForPreExaminationResult> Handle(GetMedicineListForPreExaminationQuery request, CancellationToken cancellationToken)
        {
            var receptionVaccinationsData = await _dbContext.ReceptionVaccinations
            .Where(rv => rv.ReceptionId == request.ReceptionId && rv.IsPreExaminationTesting)
            .Select(rv => new
            {
                rv.Id,
                rv.Reception.PatientId,
                rv.VaccineId,
                rv.DoctorId,
                rv.IsConfirmed,
                rv.VaccinationTestDate,
                rv.TestResultEntry
            })
            .ToListAsync(cancellationToken);

            if (!receptionVaccinationsData.Any())
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            // Patient Information
            var patientIds = receptionVaccinationsData.Select(rv => rv.PatientId).Distinct().ToList();
            var patientList = await _patientGrpcClient.ListPatientsByIdsAndSearchAsync(patientIds, null, cancellationToken);
            var patientDictionary = patientList.ToDictionary(p => p.Id, p => p.Name);

            //Vaccine Information
            var vaccineIds = receptionVaccinationsData.Select(rv => rv.VaccineId).Distinct().ToList();
            var vaccineInfo = await _inventoryService.GetMedicineInformationAsync(vaccineIds, cancellationToken);
            var vaccineDictionary = vaccineInfo.ToDictionary(v => v.MedicineId, v => v.MedicineName);

            //Doctor Information
            var doctorIds = receptionVaccinationsData.Select(rv => rv.DoctorId).Distinct().ToList();
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var doctorDictionary = new Dictionary<int, string>();
            foreach (var doctorId in doctorIds)
            {
                var response = await _applicationUserProtoServiceClient.GetApplicationUserAsync(
                    new GetApplicationUserRequest { Id = doctorId },
                    metadata
                );
                doctorDictionary[doctorId] = response?.Name ?? string.Empty;
            }

            var receptionVaccinations = new List<PreExaminationMedicineItem>();
            foreach (var rv in receptionVaccinationsData)
            {
                var patientName = patientDictionary.GetValueOrDefault(rv.PatientId, "");
                var vaccineName = vaccineDictionary.GetValueOrDefault(rv.VaccineId, "");

                receptionVaccinations.Add(new PreExaminationMedicineItem(
                    ReceptionVaccinationId: rv.Id,
                    PatientName: patientName,
                    VaccineName: vaccineName??"",
                    IsConfirmed: rv.IsConfirmed,
                    VaccinationTestDate: rv.VaccinationTestDate ?? DateTime.MinValue,
                    TestResultEntry: rv.TestResultEntry ?? string.Empty,
                    DoctorName: doctorDictionary.TryGetValue(rv.DoctorId, out var doctorName) ? doctorName : ""
                ));
            }

            return new GetMedicineListForPreExaminationResult(receptionVaccinations);
        }
    }
}
