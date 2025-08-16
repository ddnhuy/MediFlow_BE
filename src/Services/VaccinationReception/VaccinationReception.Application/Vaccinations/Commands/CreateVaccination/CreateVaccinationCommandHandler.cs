using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Drawing.Slicer.Style;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Vaccinations.Commands.CreateVaccination
{
    public class CreateVaccinationCommandHandler : ICommandHandler<CreateVaccinationCommand, CreateVaccinationResponse>
    {
        private const string POSITIVE_RESULT = "positive";
        private readonly IApplicationDbContext _dbContext;
        private readonly IInventoryService _inventoryService;

        public CreateVaccinationCommandHandler(IApplicationDbContext dbContext, IInventoryService inventoryService)
        {
            _dbContext = dbContext;
            _inventoryService = inventoryService;
        }

        public async Task<CreateVaccinationResponse> Handle(CreateVaccinationCommand request, CancellationToken cancellationToken)
        {
            // Get all existing doses for this ReceptionVaccination
            var existingDoses = await _dbContext.Vaccinations
                .Where(v => v.PatientId == request.PatientId && v.MedicineId == request.MedicineId)
                .ToListAsync(cancellationToken);

            var receptionVaccination = await _dbContext.ReceptionVaccinations
                .Include(rv => rv.Reception)
                .Include(rv => rv.SecondaryReception)
                .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVaccination == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            if (receptionVaccination.HasIssue)
            {
                throw new BadRequestException(ExceptionKey.CANNOT_TAKE_ISSUE_VACCINE_IF_HAS_ISSUE);
            }

            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync([request.MedicineId], cancellationToken);

            var medicineInformation = medicineInformationList.FirstOrDefault(m => m.MedicineId == request.MedicineId);

            if (medicineInformation!.IsRequiredTestingBeforeUse == true)
            {
                if (string.IsNullOrEmpty(receptionVaccination.TestResultEntry) || receptionVaccination.IsPreExaminationTesting == false)
                {
                    throw new BadRequestException(ExceptionKey.VACCINE_REQUIRED_PRE_EXAMINATION_TESTING_BEFORE_VACCINATION);
                }
                else if (receptionVaccination.TestResultEntry == POSITIVE_RESULT)
                {
                    throw new BadRequestException(ExceptionKey.CANNOT_TAKE_VACCINATION_IF_RESULT_IS_POSITIVE);
                }
            }

            // Check if the number of doses already equals or exceeds the allowed quantity
            var existingDosesForThisReception = existingDoses.Where(v => v.ReceptionVaccinationId == request.ReceptionVaccinationId).ToList();

            if (existingDosesForThisReception.Count >= receptionVaccination!.Quantity)
            {
                throw new BadRequestException(ExceptionKey.ENOUGH_VACCINATION_DOSE_FOR_VACCINATION_RECEPTION);
            }

            int nextDoseNumber = existingDoses.Count + 1;

            var vaccination = new Vaccination
            {
                PatientId = request.PatientId,
                ReceptionVaccinationId = request.ReceptionVaccinationId,
                MedicineBatchId = request.MedicineId,
                BatchNumber = request.BatchNumber,
                MedicineId = request.MedicineId,
                MedicineName = request.MedicineName,
                Note = request.Note,
                DoctorId = request.DoctorId,
                VaccinationDate = DateTime.UtcNow,
                IsConfirmed = true,
                DoseNumber = nextDoseNumber
            };

            var currentReception = receptionVaccination.SecondaryReception ?? receptionVaccination.Reception;

            if (currentReception == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            }
            else
            {
                currentReception.LastUpdatedAt = DateTime.UtcNow;
            }

            _dbContext.Vaccinations.Add(vaccination);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _inventoryService.SubtractMedicineBatchStockResponseAsync(request.MedicineBatchId, 1, cancellationToken);

            return new CreateVaccinationResponse(vaccination.Id);
        }
    }
}
