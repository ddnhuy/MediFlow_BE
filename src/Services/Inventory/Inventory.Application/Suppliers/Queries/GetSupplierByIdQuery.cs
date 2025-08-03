namespace Inventory.Application.Suppliers.Queries.GetSupplierById
{
    public record GetSupplierByIdQuery(int Id) : IQuery<GetSupplierByIdResult>;
    public record GetSupplierByIdResult(SupplierDetailDTO Supplier);
}