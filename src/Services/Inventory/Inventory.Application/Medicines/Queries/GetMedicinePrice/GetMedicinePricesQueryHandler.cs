namespace Inventory.Application.Medicines.Queries.GetMedicinePrice
{
    public class GetMedicinePricesQueryHandler(IApplicationDbContext dbContext)
       : IQueryHandler<GetMedicinePricesQuery, GetMedicinePricesResult>
    {
        public async Task<GetMedicinePricesResult> Handle(GetMedicinePricesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            // Get all active medicines
            var medicines = await dbContext.Medicines
                .Where(m => !m.IsSuspended && !m.IsCancelled)
                .OrderBy(m => m.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var medicineIds = medicines.Select(m => m.Id).ToList();

            // Get all prices for these medicines
            var medicinePrices = await dbContext.MedicinePrices
                .Where(mp => medicineIds.Contains(mp.MedicineId) && !mp.IsCancelled)
                .Include(mp => mp.Medicine)
                .OrderBy(mp => mp.MedicineId)
                .OrderByDescending(mp => mp.LastUpdatedAt)
                .ToListAsync(cancellationToken);

            // Group prices by medicine ID
            var pricesByMedicineId = medicinePrices.GroupBy(mp => mp.MedicineId).ToDictionary(g => g.Key, g => g.ToList());

            var medicinePriceDTOs = new List<MedicinePriceDTO>();

            foreach (var medicine in medicines)
            {
                if (pricesByMedicineId.TryGetValue(medicine.Id, out var prices))
                {
                    // Add all prices for this medicine
                    foreach (var price in prices)
                    {
                        medicinePriceDTOs.Add(new MedicinePriceDTO
                        {
                            Id = price.Id,
                            MedicineId = medicine.Id,
                            MedicineName = medicine.MedicineName ?? string.Empty,
                            UnitPrice = price.UnitPrice,
                            Currency = price.Currency,
                            VatRate = price.VatRate,
                            VatAmount = price.VatAmount,
                            OriginalPriceAfterVat = price.OriginalPriceAfterVat,
                            OriginalPriceBeforeVat = price.OriginalPriceBeforeVat,
                            IsSuspended = price.IsSuspended,
                            IsCancelled = price.IsCancelled,
                            CreatedAt = price.CreatedAt,
                            CreatedBy = price.CreatedBy,
                            LastUpdatedAt = price.LastUpdatedAt,
                            LastUpdatedBy = price.LastUpdatedBy
                        });
                    }
                }
                else
                {
                    // Add medicine with null price values
                    medicinePriceDTOs.Add(new MedicinePriceDTO
                    {
                        Id = 0,
                        MedicineId = medicine.Id,
                        MedicineName = medicine.MedicineName ?? string.Empty,
                        UnitPrice = null,
                        Currency = null,
                        VatRate = null,
                        VatAmount = null,
                        OriginalPriceAfterVat = null,
                        OriginalPriceBeforeVat = null,
                        IsSuspended = false,
                        IsCancelled = false,
                        CreatedAt = medicine.CreatedAt,
                        CreatedBy = medicine.CreatedBy,
                        LastUpdatedAt = medicine.LastUpdatedAt,
                        LastUpdatedBy = medicine.LastUpdatedBy
                    });
                }
            }

            // Get total count for pagination
            var totalCount = await dbContext.Medicines
                .Where(m => !m.IsSuspended && !m.IsCancelled)
                .LongCountAsync(cancellationToken);

            return new GetMedicinePricesResult(
                new PaginatedResult<MedicinePriceDTO>(pageIndex, pageSize, totalCount, medicinePriceDTOs));
        }
    }
}