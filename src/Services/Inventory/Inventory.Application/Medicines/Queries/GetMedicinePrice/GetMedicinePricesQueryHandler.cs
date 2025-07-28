namespace Inventory.Application.Medicines.Queries.GetMedicinePrice
{
    public class GetMedicinePricesQueryHandler(IApplicationDbContext dbContext)
       : IQueryHandler<GetMedicinePricesQuery, GetMedicinePricesResult>
    {
        public async Task<GetMedicinePricesResult> Handle(GetMedicinePricesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await dbContext.MedicinePrices
                .Where(p => !p.IsSuspended && !p.IsCancelled)
                .LongCountAsync(cancellationToken);

            var medicinePrices = await dbContext.MedicinePrices
                .Where(p => !p.IsSuspended && !p.IsCancelled)
                .Include(p => p.Medicine)
                .OrderBy(p => p.MedicineId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var medicinePriceDTOs = medicinePrices.Select(mp => new MedicinePriceDTO
            {
                Id = mp.Id,
                MedicineId = mp.MedicineId,
                MedicineName = mp.Medicine?.MedicineName ?? string.Empty,
                UnitPrice = mp.UnitPrice,
                Currency = mp.Currency,
                VatRate = mp.VatRate,
                VatAmount = mp.VatAmount,
                OriginalPriceAfterVat = mp.OriginalPriceAfterVat,
                OriginalPriceBeforeVat = mp.OriginalPriceBeforeVat,
                IsSuspended = mp.IsSuspended,
                IsCancelled = mp.IsCancelled,
                CreatedAt = mp.CreatedAt,
                CreatedBy = mp.CreatedBy,
                LastUpdatedAt = mp.LastUpdatedAt,
                LastUpdatedBy = mp.LastUpdatedBy
            }).ToList();

            return new GetMedicinePricesResult(
                new PaginatedResult<MedicinePriceDTO>(pageIndex, pageSize, totalCount, medicinePriceDTOs));
        }
    }
}
