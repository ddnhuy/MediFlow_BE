using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Strings;
using Microsoft.EntityFrameworkCore;
using VaccinationReception.Application.Abstraction.InventoryMessaging;
using VaccinationReception.Application.Data;

namespace VaccinationReception.Application.VaccinationReceptions.Commands
{
    public class AddVaccineToPreExaminationTestingCommandHandler : ICommandHandler<AddVaccineToPreExaminationTestingCommand, AddVaccineToPreExaminationTestingResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IInventoryService _inventoryService;

        public AddVaccineToPreExaminationTestingCommandHandler(IApplicationDbContext dbContext, IInventoryService inventoryService)
        {
            _dbContext = dbContext;
            _inventoryService = inventoryService;
        }

        public async Task<AddVaccineToPreExaminationTestingResult> Handle(AddVaccineToPreExaminationTestingCommand request, CancellationToken cancellationToken)
        {
            var receptionVaccination = await _dbContext.ReceptionVaccinations.FirstOrDefaultAsync( x => x.Id == request.ReceptionVaccinationId, cancellationToken);

            if (receptionVaccination == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_VACCINATION_RECEPTION_WITH_ID);
            }

            var vaccineInfo = await _inventoryService.GetMedicineInformationAsync([receptionVaccination.VaccineId], cancellationToken);

            var IsRequiredPreExaminationTesting = vaccineInfo.FirstOrDefault()?.IsRequiredTestingBeforeUse ?? false;

            if (IsRequiredPreExaminationTesting == false)
            {
                throw new BadRequestException(ExceptionKey.VACCINE_NOT_REQUIRED_PRE_EXAMINATION_TESTING);
            }

            receptionVaccination.IsPreExaminationTesting = true;
            receptionVaccination.VaccinationTestDate = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new AddVaccineToPreExaminationTestingResult(IsSuccess: true);
        }
    }
}
