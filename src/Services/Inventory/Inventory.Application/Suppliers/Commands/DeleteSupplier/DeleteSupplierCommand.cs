namespace Inventory.Application.Suppliers.Commands.DeleteSupplier
{
    public record DeleteSupplierCommand(int Id) : ICommand<DeleteSupplierResult>;
    public record DeleteSupplierResult(bool IsSuccess);
}
