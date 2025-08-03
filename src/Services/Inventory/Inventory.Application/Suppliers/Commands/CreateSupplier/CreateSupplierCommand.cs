namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public record CreateSupplierCommand(
        string SupplierName,
        string Address,
        string Phone,
        string Fax,
        string Email,
        string TaxCode,
        string Director,
        string ContactPerson,
        DateOnly ExpiredDate,
        List<CreateSupplierContractRequest> Contracts
    ) : ICommand<CreateSupplierResult>;

    public record CreateSupplierContractRequest(
        Guid Id,
        string FileName
    );

    public record CreateSupplierResult(int Id);
}
