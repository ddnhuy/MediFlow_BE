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

            var medicinePriceDTOs = medicinePrices.Adapt<List<MedicinePriceDTO>>();

            return new GetMedicinePricesResult(
                new PaginatedResult<MedicinePriceDTO>(pageIndex, pageSize, totalCount, medicinePriceDTOs));
        }
    }
}
