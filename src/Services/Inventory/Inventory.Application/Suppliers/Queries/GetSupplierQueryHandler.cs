namespace Inventory.Application.Suppliers.Queries
{
    public class GetSupplierQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetSupplierQuery, GetSupplierResult>
    {
        public async Task<GetSupplierResult> Handle(GetSupplierQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.Request.PageIndex;
            var pageSize = request.Request.PageSize;
            
            var totalCounts = await dbContext.Suppliers
                .Where(x => !x.IsCancelled)
                .LongCountAsync(cancellationToken);

            var suppliers = await dbContext.Suppliers.Where(x => !x.IsCancelled)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var supplierDTOs = suppliers.Adapt<List<SupplierDTO>>();

            return new GetSupplierResult(new PaginatedResult<SupplierDTO>(pageIndex, pageSize, totalCounts, supplierDTOs));
        }
    }
}
