using BuildingBlocks.CQRS;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Application.Services.PatientServices;

namespace VaccinationReception.Application.Vaccinations.Queries.GetVaccinationHistoryByPatientId
{
    public class GetVaccinationHistoryByPatientIdQueryHandler: IQueryHandler<GetVaccinationHistoryByPatientIdQuery, GetVaccinationHistoryByPatientIdResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IPatientGrpcClient _patientGrpcClient;
        private readonly IInventoryService _inventoryService;

        public GetVaccinationHistoryByPatientIdQueryHandler(IApplicationDbContext dbContext, IPatientGrpcClient patientGrpcClient, IInventoryService inventoryService)
        {
            _dbContext = dbContext;
            _patientGrpcClient = patientGrpcClient;
            _inventoryService = inventoryService;
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

            var vaccinationHistoryItems = vaccinationHistory.Select(v =>
            {
                var medicineInfo = medicineInfoDict.GetValueOrDefault(v.MedicineId);

                return new VaccinationHistoryItem(
                    Id: v.Id,
                    MedicineTypeName: medicineInfo?.VaccineTypeName ?? string.Empty,
                    MedicineName: v.MedicineName ?? string.Empty,
                    Concentration: medicineInfo?.Concentration ?? string.Empty,
                    VaccinationTestDate: v.ReceptionVaccination?.VaccinationTestDate,
                    VaccinationDate: v.VaccinationDate,
                    VaccinationStatus: v.VaccinationConfirmation ?? string.Empty,
                    DoctorName: v.DoctorName ?? string.Empty
                );
            }).ToList();

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
            );

            return history;
        }
    }
}
