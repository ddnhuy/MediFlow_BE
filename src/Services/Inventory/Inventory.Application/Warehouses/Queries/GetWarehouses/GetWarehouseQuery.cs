namespace Inventory.Application.Warehouses.Queries.GetWarehouses
{
    public record GetWarehouseQuery(PaginationRequest PaginationRequest) : IQuery<GetWarehouseResult>;

    public record GetWarehouseResult(PaginatedResult<WarehouseDTO> Warehouses);
}