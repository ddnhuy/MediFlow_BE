using BuildingBlocks.CQRS;
using BuildingBlocks.Pagination;
using Inventory.Application.DTOs;

namespace Inventory.Application.Warehouses.Queries.GetWarehouses
{
    public record GetWarehouseQuery(PaginationRequest PaginationRequest) : IQuery<GetWarehouseResult>;

    public record GetWarehouseResult(PaginatedResult<WarehouseDTO> Warehouses);
}