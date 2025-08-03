namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public record UpdateSupplierCommand(
        int Id,
        string SupplierName,
        string Phone,
        string Fax, 
        string Email,
        string TaxCode,
        string Address,
        string ContactPerson,
        string Director,
        bool IsSuspended,
        bool IsCancelled,
        DateOnly ExpiredDate,
        List<UpdateSupplierContractRequest> Contracts
    ) : ICommand<UpdateSupplierResult>;

    public record UpdateSupplierContractRequest(
        Guid Id,
        string FileName
    );

    public record UpdateSupplierResult(bool IsSuccess);
}
