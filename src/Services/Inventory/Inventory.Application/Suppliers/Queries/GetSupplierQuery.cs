namespace Inventory.Application.Suppliers.Queries
{
    public record GetSupplierQuery(PaginationRequest Request) : IQuery<GetSupplierResult>;
    public record GetSupplierResult(PaginatedResult<SupplierDTO> Suppliers);
}