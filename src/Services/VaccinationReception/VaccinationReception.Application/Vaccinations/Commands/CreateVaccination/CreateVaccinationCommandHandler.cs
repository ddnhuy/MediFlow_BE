using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Vaccinations.Commands.CreateVaccination
{
    public class CreateVaccinationCommandHandler : ICommandHandler<CreateVaccinationCommand, CreateVaccinationResponse>
    {
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
                .Where(v => v.ReceptionVaccinationId == request.ReceptionVaccinationId)
                .ToListAsync(cancellationToken);

            var receptionVaccination = await _dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVaccination == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            var medicineInformationList = await _inventoryService.GetMedicineInformationAsync([request.MedicineId], cancellationToken);

            var medicineInformation = medicineInformationList.FirstOrDefault(m => m.MedicineId == request.MedicineId);

            if (medicineInformation!.IsRequiredTestingBeforeUse == true 
                && (receptionVaccination!.IsPreExaminationTesting == false || string.IsNullOrEmpty(receptionVaccination.TestResultEntry)))
            { 
                throw new BadRequestException(ExceptionKey.VACCINE_REQUIRED_PRE_EXAMINATION_TESTING_BEFORE_VACCINATION);
            }

            // Check if the number of doses already equals or exceeds the allowed quantity
            if (existingDoses.Count >= receptionVaccination!.Quantity)
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

            _dbContext.Vaccinations.Add(vaccination);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateVaccinationResponse(vaccination.Id);
        }
    }
}
