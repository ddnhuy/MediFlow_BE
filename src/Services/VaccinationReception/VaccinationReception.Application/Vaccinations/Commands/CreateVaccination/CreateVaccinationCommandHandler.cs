using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
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
                Note = request.Note,
                DoctorId = request.DoctorId,
                VaccinationDate = DateTime.UtcNow
            };

            // Update the status of reception vaccination
            var receptionVaccination = await _dbContext.ReceptionVaccinations
                .FirstOrDefaultAsync(rv => rv.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVaccination == null)
            {
                throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.NOT_FOUND_RECEPTION_WITH_ID);
            } else
            {
                receptionVaccination.IsConfirmed = true;
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
                         
            _dbContext.Vaccinations.Add(vaccination);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new CreateVaccinationResponse(vaccination.Id);
        }
    }
}
