using Inventory.Application.Helpers;

namespace Inventory.Application.Medicines.Queries.GetMedicineById
{
    public class GetMedicineByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineByIdQuery, GetMedicineByIdResult>
    {
        public async Task<GetMedicineByIdResult> Handle(GetMedicineByIdQuery request, CancellationToken cancellationToken)
        {
            var medicine = await dbContext.Medicines
                .Include(m => m.VaccineType)
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
                .OrderByDescending(mp => mp.LastUpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var currentStock = await dbContext.InventoryDetails
                .Where(id => !id.IsSuspended
                    && !id.IsCancelled
                    && dbContext.MedicineBatches
                        .Any(mb => mb.Id == id.MedicineBatchId
                            && mb.MedicineId == request.Id
                            && !mb.IsSuspended
                            && !mb.IsCancelled
                            && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)))
                .SumAsync(id => id.Quantity, cancellationToken);

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
                VaccineTypeName = medicine.VaccineType?.VaccineTypeName,
                CreatedAt = medicine.CreatedAt,
                IsCancelled = medicine.IsCancelled,
                LastUpdatedAt = medicine.LastUpdatedAt,
                CreatedBy = medicine.CreatedBy,
                IsRequiredTestingBeforeUse = medicine.IsRequiredTestingBeforeUse,
                UnitPrice = latestPrice?.UnitPrice,
                LastUpdatedBy = medicine.LastUpdatedBy,
                CurrentStock = currentStock
            };

            return new GetMedicineByIdResult(medicineDTO);
        }
    }
}
