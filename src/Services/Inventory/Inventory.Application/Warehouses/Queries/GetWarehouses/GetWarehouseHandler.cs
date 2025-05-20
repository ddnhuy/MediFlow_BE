using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Inventory.Application.Data;
using Inventory.Application.DTOs;
using Mapster;

namespace Inventory.Application.Warehouses.Queries.GetWarehouses
{
    public class GetWarehouseHandler(IApplicationDbContext dbContext) : IQueryHandler<GetWarehouseQuery, GetWarehouseResult>
    {
        public async Task<GetWarehouseResult> Handle(GetWarehouseQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCounts = await dbContext.Warehouses.LongCountAsync(cancellationToken: cancellationToken);

            var warehouses = await dbContext.Warehouses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            var warehouseDTOs = warehouses.Adapt<List<WarehouseDTO>>();

            return new GetWarehouseResult(new PaginatedResult<WarehouseDTO>(pageIndex, pageSize, totalCounts, warehouseDTOs));
        }
    }
}
