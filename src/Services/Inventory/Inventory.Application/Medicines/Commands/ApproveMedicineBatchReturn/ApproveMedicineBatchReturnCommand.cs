namespace Inventory.Application.Medicines.Commands.ApproveMedicineBatchReturn
{
    public record ApproveMedicineBatchReturnCommand(int Id, string Token) : ICommand<ApproveMedicineBatchReturnResult>;

    public record ApproveMedicineBatchReturnResult(bool Success);
}
