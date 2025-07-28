using Microsoft.EntityFrameworkCore;

namespace Inventory.Application.Medicines.Commands.UpdateMedicine
{
    public class UpdateMedicineCommandHandler : ICommandHandler<UpdateMedicineCommand, UpdateMedicineResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public UpdateMedicineCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UpdateMedicineResult> Handle(UpdateMedicineCommand request, CancellationToken cancellationToken)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (medicine == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            // Check if the medicine code already exists for another medicine
            if (await _dbContext.Medicines.AnyAsync(m => m.MedicineCode!.Trim().ToLower() == request.MedicineCode.Trim().ToLower(), cancellationToken))
            {
                throw new BadRequestException(ExceptionKey.DUPLICATE_MEDICINE_CODE);
            }

            medicine.MedicineCode = request.MedicineCode;
            medicine.MedicineName = request.MedicineName;
            medicine.Unit = request.Unit;
            medicine.IsRequiredTestingBeforeUse = request.IsRequiredTestingBeforeUse;
            medicine.ActiveIngredient = request.ActiveIngredient;
            medicine.UsageInstructions = request.UsageInstructions;
            medicine.Concentration = request.Concentration;
            medicine.Indications = request.Indications;
            medicine.MedicineClassification = request.MedicineClassification;
            medicine.RouteOfAdministration = request.RouteOfAdministration;
            medicine.NationalMedicineCode = request.NationalMedicineCode;
            medicine.Description = request.Description;
            medicine.Note = request.Note;
            medicine.RegistrationNumber = request.RegistrationNumber;
            medicine.VaccineTypeId = request.VaccineTypeId;
            medicine.IsSuspended = request.IsSuspended;
            medicine.IsCancelled = request.IsCancelled;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateMedicineResult(true);
        }
    }
}
