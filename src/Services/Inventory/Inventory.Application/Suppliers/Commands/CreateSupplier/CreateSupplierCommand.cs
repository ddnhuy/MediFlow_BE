namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public record CreateSupplierCommand(
        string SupplierCode,
        string SupplierName,
        string Address,
        string Phone,
        string Fax,
        string Email,
        string TaxCode,
        string Director,
        string ContactPerson
    ): ICommand<CreateSupplierResult>;
    public record CreateSupplierResult(int Id);
}
