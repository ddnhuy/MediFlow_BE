using BuildingBlocks.Strings.Enums;
using Inventory.Application.Helpers;

namespace Inventory.Application.Medicines.Queries.GetMedicines
{
    public class GetMedicineQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicinesQuery, GetMedicinesResult>
    {
        public async Task<GetMedicinesResult> Handle(GetMedicinesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var baseQuery = dbContext.Medicines
                .Include(m => m.VaccineType)
                .Where(x => !x.IsCancelled);

            if (!string.IsNullOrWhiteSpace(query.SearchKeyword))
            {
                var searchKeyword = query.SearchKeyword.ToLower();
                baseQuery = baseQuery.Where(m =>
                    (m.MedicineName != null && m.MedicineName.ToLower().Contains(searchKeyword)) ||
                    (m.MedicineCode != null && m.MedicineCode.ToLower().Contains(searchKeyword))
                );
            }

            var totalCounts = await baseQuery.LongCountAsync(cancellationToken: cancellationToken);

            var medicines = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            var medicineIds = medicines.Select(m => m.Id).ToList();

            var medicinePrices = await dbContext.MedicinePrices
                .Where(mp => medicineIds.Contains(mp.MedicineId) && !mp.IsSuspended && !mp.IsCancelled)
                .GroupBy(mp => mp.MedicineId)
                .Select(g => new { MedicineId = g.Key, UnitPrice = g.OrderByDescending(p => p.LastUpdatedAt).First().UnitPrice })
                .ToListAsync(cancellationToken: cancellationToken);

            var priceLookup = medicinePrices.ToDictionary(p => p.MedicineId, p => p.UnitPrice);

            // Calculate current stock for each medicine
            var currentStocks = await dbContext.Medicines
                .Where(m => medicineIds.Contains(m.Id))
                .Select(m => new
                {
                    MedicineId = m.Id,
                    CurrentStock = dbContext.InventoryDetails
                        .Where(id => !id.IsSuspended
                            && !id.IsCancelled
                            && dbContext.MedicineBatches
                                .Any(mb => mb.Id == id.MedicineBatchId
                                    && mb.MedicineId == m.Id
                                    && mb.Status == MedicineBatchStatus.IsActive
                                    && !mb.IsSuspended
                                    && !mb.IsCancelled
                                    && mb.ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow)))
                        .Sum(id => id.Quantity)
                })
                .ToListAsync(cancellationToken: cancellationToken);

            var stockLookup = currentStocks.ToDictionary(s => s.MedicineId, s => s.CurrentStock);

            var medicineDTOs = medicines.Select(medicine => new MedicineDTO
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
                RouteOfAdministration = EnumHelper.ToEnumString(medicine.RouteOfAdministration),
                NationalMedicineCode = medicine.NationalMedicineCode,
                Description = medicine.Description,
                Note = medicine.Note,
                RegistrationNumber = medicine.RegistrationNumber,
                MedicineTypeId = medicine.MedicineTypeId ?? 0,
                VaccineTypeId = medicine.VaccineTypeId ?? 0,
                VaccineTypeName = medicine.VaccineType?.VaccineTypeName,
                IsSuspended = medicine.IsSuspended,
                IsCancelled = medicine.IsCancelled,
                CreatedAt = medicine.CreatedAt,
                CreatedBy = medicine.CreatedBy,
                LastUpdatedAt = medicine.LastUpdatedAt,
                LastUpdatedBy = medicine.LastUpdatedBy,
                UnitPrice = priceLookup.ContainsKey(medicine.Id) ? priceLookup[medicine.Id] : null,
                IsRequiredTestingBeforeUse = medicine.IsRequiredTestingBeforeUse ?? false,
                CurrentStock = stockLookup.ContainsKey(medicine.Id) ? stockLookup[medicine.Id] : 0
            }).OrderByDescending(x => x.CurrentStock).ToList();

            return new GetMedicinesResult(new PaginatedResult<MedicineDTO>(pageIndex, pageSize, totalCounts, medicineDTOs));
        }
    }
}
