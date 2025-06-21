using BuildingBlocks.CQRS;
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

            var vaccination = new Vaccination
            {
                PatientId = request.PatientId,
                ReceptionVaccinationId = request.ReceptionVaccinationId,
                MedicineBatchId = request.MedicineId,
                BatchNumber = request.BatchNumber,
                MedicineId = request.MedicineId,
                MedicineName = request.MedicineName,
                VaccinationConfirmation = request.VaccinationConfirmation,
                ScheduleVaccinationDate = request.ScheduleVaccinationDate,
                Note = request.Note,
                DoctorId = request.DoctorId,
                DoctorName = request.DoctorName,
                VaccinationDate = request.VaccinationDate
            };

            _dbContext.Vaccinations.Add(vaccination);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateVaccinationResponse(vaccination.Id);
        }
    }
}
