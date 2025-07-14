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
                .Where(x => !x.IsSuspended && !x.IsCancelled);

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
                .Select(g => new { MedicineId = g.Key, UnitPrice = g.OrderByDescending(p => p.CreatedAt).First().UnitPrice })
                .ToListAsync(cancellationToken: cancellationToken);

            var priceLookup = medicinePrices.ToDictionary(p => p.MedicineId, p => p.UnitPrice);

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
                IsSuspended = medicine.IsSuspended,
                IsCancelled = medicine.IsCancelled,
                CreatedAt = medicine.CreatedAt,
                CreatedBy = medicine.CreatedBy,
                LastUpdatedAt = medicine.LastUpdatedAt,
                LastUpdatedBy = medicine.LastUpdatedBy,
                UnitPrice = priceLookup.ContainsKey(medicine.Id) ? priceLookup[medicine.Id] : null
            }).ToList();

            return new GetMedicinesResult(new PaginatedResult<MedicineDTO>(pageIndex, pageSize, totalCounts, medicineDTOs));
        }
    }
}
