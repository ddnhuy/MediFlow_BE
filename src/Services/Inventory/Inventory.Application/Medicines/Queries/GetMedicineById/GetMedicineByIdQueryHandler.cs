namespace Inventory.Application.Medicines.Queries.GetMedicineById
{
    public class GetMedicineByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineByIdQuery, GetMedicineByIdResult>
    {
        public async Task<GetMedicineByIdResult> Handle(GetMedicineByIdQuery request, CancellationToken cancellationToken)
        {
            var medicine = await dbContext.Medicines
                .Where(x => !x.IsSuspended && !x.IsCancelled)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (medicine == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            // Get the latest price for this medicine
            var latestPrice = await dbContext.MedicinePrices
                .Where(mp => mp.MedicineId == request.Id && !mp.IsSuspended && !mp.IsCancelled)
                .OrderByDescending(mp => mp.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            // Convert to DTO and include unit price
            var medicineDTO = new MedicineDTO
            {
                Id = medicine.Id,
                MedicineCode = medicine.MedicineCode,
                MedicineName = medicine.MedicineName,
                Unit = medicine.Unit,
                ActiveIngredient = medicine.ActiveIngredient,
                UsageInstructions = medicine.UsageInstructions,
                Concentration = medicine.Concentration,
                Indications = medicine.Indications,
                MedicineClassification = medicine.MedicineClassification,
                RouteOfAdministration = medicine.RouteOfAdministration,
                NationalMedicineCode = medicine.NationalMedicineCode,
                Description = medicine.Description,
                Note = medicine.Note,
                RegistrationNumber = medicine.RegistrationNumber,
                MedicineTypeId = medicine.MedicineTypeId ?? 0,
                VaccineTypeId = medicine.VaccineTypeId ?? 0,
                IsSuspended = medicine.IsSuspended,
                IsCancelled = medicine.IsCancelled,
                CreatedAt = medicine.CreatedAt,
                CreatedBy = medicine.CreatedBy,
                LastUpdatedAt = medicine.LastUpdatedAt,
                LastUpdatedBy = medicine.LastUpdatedBy,
                UnitPrice = latestPrice?.UnitPrice
            };

            return new GetMedicineByIdResult(medicineDTO);
        }
    }
}
