namespace Inventory.Application.Suppliers.Queries
{
    public class GetSupplierQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetSupplierQuery, GetSupplierResult>
    {
        public async Task<GetSupplierResult> Handle(GetSupplierQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.Request.PageIndex;
            var pageSize = request.Request.PageSize;

            var baseQuery = dbContext.Suppliers.Where(x => !x.IsCancelled);

            if (!string.IsNullOrWhiteSpace(request.searchTerm))
            {
                var searchKeyword = request.searchTerm.ToLower();
                baseQuery = baseQuery.Where(s =>
                    (s.SupplierName != null && s.SupplierName.ToLower().Contains(searchKeyword)) ||
                    (s.SupplierCode != null && s.SupplierCode.ToLower().Contains(searchKeyword)));
            }

            var totalCounts = await baseQuery.LongCountAsync(cancellationToken);

            var suppliers = await baseQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var supplierDTOs = suppliers.Adapt<List<SupplierDTO>>();

            return new GetSupplierResult(new PaginatedResult<SupplierDTO>(pageIndex, pageSize, totalCounts, supplierDTOs));
        }
    }
}
