namespace Inventory.Application.Medicines.Commands.RejectMedicineBatchReturn
{
    public record RejectMedicineBatchReturnCommand(int Id, string Token) : ICommand<RejectMedicineBatchReturnResult>;

    public record RejectMedicineBatchReturnResult(bool Success);
}
