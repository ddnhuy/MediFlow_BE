namespace Inventory.Application.Suppliers.Queries
{
    public record GetSupplierQuery(PaginationRequest Request, string? searchTerm = null) : IQuery<GetSupplierResult>;
    public record GetSupplierResult(PaginatedResult<SupplierDTO> Suppliers);
}