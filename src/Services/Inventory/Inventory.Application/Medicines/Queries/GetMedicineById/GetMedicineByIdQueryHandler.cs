using Inventory.Application.Helpers;

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
                MedicineCode = medicine.MedicineCode,
                Id = medicine.Id,
                Unit = medicine.Unit,
                MedicineName = medicine.MedicineName,
                UsageInstructions = medicine.UsageInstructions,
                ActiveIngredient = medicine.ActiveIngredient,
                Indications = medicine.Indications,
                Concentration = medicine.Concentration,
                NationalMedicineCode = medicine.NationalMedicineCode,
                MedicineClassification = medicine.MedicineClassification,
                RouteOfAdministration = EnumHelper.ToEnumString(medicine.RouteOfAdministration),
                Note = medicine.Note,
                Description = medicine.Description,
                MedicineTypeId = medicine.MedicineTypeId ?? 0,
                RegistrationNumber = medicine.RegistrationNumber,
                IsSuspended = medicine.IsSuspended,
                VaccineTypeId = medicine.VaccineTypeId ?? 0,
                CreatedAt = medicine.CreatedAt,
                IsCancelled = medicine.IsCancelled,
                LastUpdatedAt = medicine.LastUpdatedAt,
                CreatedBy = medicine.CreatedBy,
                UnitPrice = latestPrice?.UnitPrice,
                LastUpdatedBy = medicine.LastUpdatedBy,
            };

            return new GetMedicineByIdResult(medicineDTO);
        }
    }
}
