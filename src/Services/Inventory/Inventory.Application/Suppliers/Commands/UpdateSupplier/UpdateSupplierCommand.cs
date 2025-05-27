namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public record UpdateSupplierCommand(
        int Id,
        string SupplierCode,
        string SupplierName,
        string Phone,
        string Fax, 
        string Email,
        string TaxCode,
        string Address,
        string ContactPerson,
        string Director,
        bool IsSuspended,
        bool IsCancelled
    ) : ICommand<UpdateSupplierResult>;

    public record UpdateSupplierResult(bool IsSuccess);
}
