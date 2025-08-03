namespace Inventory.Application.Medicines.Queries.GetMedicinePricesByMedicineId
{
    public class GetMedicinePricesByMedicineIdQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicinePricesByMedicineIdQuery, GetMedicinePricesByMedicineIdResult>
    {
        public async Task<GetMedicinePricesByMedicineIdResult> Handle(
            GetMedicinePricesByMedicineIdQuery query, CancellationToken cancellationToken)
        {
            var medicines = await dbContext.Medicines
                .Where(m => m.Id == query.MedicineId && !m.IsSuspended && !m.IsCancelled)
                .OrderByDescending(x => x.LastUpdatedAt)
                .ToListAsync(cancellationToken);

            if (!medicines.Any())
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            var medicinePrices = await dbContext.MedicinePrices
                .Where(p => p.MedicineId == query.MedicineId && !p.IsCancelled)
                .Include(p => p.Medicine)
                .ToListAsync(cancellationToken);

            if (!medicinePrices.Any())
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_PRICE);
            }

            var medicinePriceDTOs = medicinePrices.Select(mp => new MedicinePriceDTO
            {
                OriginalPriceBeforeVat = mp.OriginalPriceBeforeVat,
                MedicineId = mp.MedicineId,
                Id = mp.Id,
                UnitPrice = mp.UnitPrice,
                MedicineName = mp.Medicine?.MedicineName ?? string.Empty,
                VatRate = mp.VatRate,
                Currency = mp.Currency,
                OriginalPriceAfterVat = mp.OriginalPriceAfterVat,
                VatAmount = mp.VatAmount,
                IsSuspended = mp.IsSuspended,
                IsCancelled = mp.IsCancelled,
                CreatedAt = mp.CreatedAt,
                LastUpdatedBy = mp.LastUpdatedBy,
                LastUpdatedAt = mp.LastUpdatedAt,
                CreatedBy = mp.CreatedBy,
            }).ToList();

            return new GetMedicinePricesByMedicineIdResult(medicinePriceDTOs);
        }
    }
}
