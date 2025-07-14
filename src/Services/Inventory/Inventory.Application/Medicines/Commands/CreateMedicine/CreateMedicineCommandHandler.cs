namespace Inventory.Application.Medicines.Commands.CreateMedicine
{
    public class CreateMedicineCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateMedicineCommand, CreateMedicineResult>
    {
        public async Task<CreateMedicineResult> Handle(CreateMedicineCommand request, CancellationToken cancellationToken)
        {
            if (await dbContext.Medicines.AnyAsync(m => m.MedicineCode == request.MedicineCode, cancellationToken))
            {
                throw new BadRequestException(ExceptionKey.DUPLICATE_MEDICINE_CODE);
            }

            var medicine = new Medicine
            {
                MedicineCode = request.MedicineCode,
                MedicineName = request.MedicineName,
                Unit = request.Unit,
                IsRequiredTestingBeforeUse = request.IsRequiredTestingBeforeUse,
                ActiveIngredient = request.ActiveIngredient,
                UsageInstructions = request.UsageInstructions,
                Concentration = request.Concentration,
                Indications = request.Indications,
                MedicineClassification = request.MedicineClassification,
                RouteOfAdministration = request.RouteOfAdministration,
                NationalMedicineCode = request.NationalMedicineCode,
                Description = request.Description,
                Note = request.Note,
                RegistrationNumber = request.RegistrationNumber,
                MedicineTypeId = request.MedicineTypeId,
                VaccineTypeId = request.VaccineTypeId,
                IsSuspended = false,
                IsCancelled = false,
            };

            var newMedicine = await dbContext.Medicines.AddAsync(medicine);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateMedicineResult(newMedicine.Entity.Id);
        }
    }
}
