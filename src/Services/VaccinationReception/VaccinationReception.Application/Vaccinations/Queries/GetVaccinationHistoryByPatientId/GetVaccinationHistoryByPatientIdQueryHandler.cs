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

            // Get ALL ReceptionVaccinations for this patient (both with and without issues)
            var allReceptionVaccinations = await _dbContext.ReceptionVaccinations
                .Include(rv => rv.Reception)
                .Include(rv => rv.SecondaryReception)
                .Where(rv => rv.Reception.PatientId == request.PatientId)
                .ToListAsync(cancellationToken);

            // Get all vaccinations for all ReceptionVaccinations
            var receptionVaccinationIds = allReceptionVaccinations.Select(rv => rv.Id).ToList();
            var allVaccinations = await _dbContext.Vaccinations
                .Where(v => receptionVaccinationIds.Contains(v.ReceptionVaccinationId))
                .ToListAsync(cancellationToken);

            // Get unique medicine IDs
            var medicineIds = allReceptionVaccinations.Select(rv => rv.VaccineId).Distinct().ToList();
            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync(medicineIds, cancellationToken);
            var medicineInfoDict = medicineInformationList.ToDictionary(m => m.MedicineId, m => m);

            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);

            var historyItems = new List<VaccinationHistoryItem>();

            foreach (var receptionVaccination in allReceptionVaccinations)
            {
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

                if (receptionVaccination.HasIssue)
                {
                    // Case 1: ReceptionVaccination has issue
                    // This could be:
                    // - Vaccine was rejected before injection
                    // - Vaccine was injected but had adverse reactions later

                    // Check if there are any vaccinations for this ReceptionVaccination
                    var vaccinationsForThisRV = allVaccinations
                        .Where(v => v.ReceptionVaccinationId == receptionVaccination.Id)
                        .ToList();

                    if (vaccinationsForThisRV.Any())
                    {
                        // Vaccine was injected but has issues (adverse reactions)
                        foreach (var vaccination in vaccinationsForThisRV)
                        {
                            historyItems.Add(new VaccinationHistoryItem(
                                Id: vaccination.Id,
                                ReceptionId: currentReceptionId,
                                ReceptionVaccinationId: receptionVaccination.Id,
                                MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                                MedicineName: vaccination.MedicineName ?? string.Empty,
                                DoseNumber: $"Mũi thứ {vaccination.DoseNumber}",
                                VaccinationTestDate: receptionVaccination.VaccinationTestDate,
                                VaccinationDate: vaccination.VaccinationDate,
                                VaccinationConfirmation: vaccination.IsConfirmed,
                                DoctorName: $"B.S {doctorName}",
                                HasIssue: true,
                                IssueNote: receptionVaccination.IssueNote,
                                IssueDate: receptionVaccination.IssueDate
                            ));
                        }
                    }
                    else
                    {
                        // Vaccine was rejected before injection
                        // Create entries based on the planned quantity
                        for (int doseNum = 1; doseNum <= receptionVaccination.Quantity; doseNum++)
                        {
                            historyItems.Add(new VaccinationHistoryItem(
                                Id: null, 
                                ReceptionId: currentReceptionId,
                                ReceptionVaccinationId: receptionVaccination.Id,
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
                else
                {
                    // Case 2: ReceptionVaccination has no issue - show actual vaccinations
                    var vaccinationsForThisRV = allVaccinations
                        .Where(v => v.ReceptionVaccinationId == receptionVaccination.Id)
                        .ToList();

                    foreach (var vaccination in vaccinationsForThisRV)
                    {
                        historyItems.Add(new VaccinationHistoryItem(
                            Id: vaccination.Id,
                            ReceptionId: currentReceptionId,
                            ReceptionVaccinationId: receptionVaccination.Id,
                            MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                            MedicineName: vaccination.MedicineName ?? string.Empty,
                            DoseNumber: $"Mũi thứ {vaccination.DoseNumber}",
                            VaccinationTestDate: receptionVaccination.VaccinationTestDate,
                            VaccinationDate: vaccination.VaccinationDate!.Value,
                            VaccinationConfirmation: vaccination.IsConfirmed,
                            DoctorName: $"B.S {doctorName}",
                            HasIssue: false,
                            IssueNote: null,
                            IssueDate: null
                        ));
                    }
                }
            }

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
                VaccinationHistoryItems: historyItems
                    .OrderBy(x => x.MedicineName)
                    .ThenByDescending(x => x.DoseNumber).ToList()
            );

            return history;
        }
    }
}