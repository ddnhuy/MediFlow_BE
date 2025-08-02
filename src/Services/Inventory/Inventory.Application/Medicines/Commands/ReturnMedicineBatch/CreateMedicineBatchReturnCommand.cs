namespace Inventory.Application.Medicines.Commands.ReturnMedicineBatch
{
    public record CreateMedicineBatchReturnCommand(
        string ReturnCode,
        string? Reason,
        string ReceiverName,
        string ReceiverEmail,
        string ReceiverPhone,
        List<MedicineBatchReturnDetailDto> Details
    ) : ICommand<CreateMedicineBatchReturnResult>;

    public record MedicineBatchReturnDetailDto(
        int MedicineBatchId,
        string BatchNumber,
        DateOnly ExpirationDate,
        decimal Quantity
    );

    public record CreateMedicineBatchReturnResult(int Id);
}
