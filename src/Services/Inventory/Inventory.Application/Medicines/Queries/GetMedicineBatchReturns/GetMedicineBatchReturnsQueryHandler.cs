namespace Inventory.Application.Medicines.Queries.GetMedicineBatchReturns
{
    public class GetMedicineBatchReturnsQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicineBatchReturnsQuery, GetMedicineBatchReturnsResult>
    {
        public async Task<GetMedicineBatchReturnsResult> Handle(GetMedicineBatchReturnsQuery request, CancellationToken cancellationToken)
        {
            var query = dbContext.MedicineBatchReturns.AsQueryable();

            // Apply search filter by return code
            if (!string.IsNullOrWhiteSpace(request.SearchReturnCode))
            {
                query = query.Where(mbr => mbr.ReturnCode.Contains(request.SearchReturnCode));
            }

            // Apply status filter
            if (request.Status.HasValue)
            {
                query = query.Where(mbr => mbr.Status == request.Status.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and ordering on the entity first, then project to DTO
            var paginatedQuery = query
                .Skip((request.Pagination.PageIndex - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .Select(mbr => new MedicineBatchReturnDto(
                    mbr.Id,
                    mbr.ReturnCode,
                    mbr.ReceiverName,
                    mbr.ReceiverPhone,
                    mbr.ReceiverEmail,
                    mbr.Status,
                    mbr.CreatedAt
                ));

            var medicineBatchReturns = await paginatedQuery.ToListAsync(cancellationToken);

            // Create paginated result
            var paginatedResult = new PaginatedResult<MedicineBatchReturnDto>(
                pageIndex: request.Pagination.PageIndex,
                pageSize: request.Pagination.PageSize,
                totalItems: totalCount,
                data: medicineBatchReturns.OrderByDescending( x => x.CreatedAt)
            );

            return new GetMedicineBatchReturnsResult(paginatedResult);
        }
    }
}