using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Queries.GetMedicineBatchReturnById
{
    public record GetMedicineBatchReturnByIdQuery(int Id) : IQuery<GetMedicineBatchReturnByIdResult>;

    public record GetMedicineBatchReturnByIdResult(MedicineBatchReturnDetailDto MedicineBatchReturn);

    public record MedicineBatchReturnDetailDto(
        int Id,
        string ReturnCode,
        string? Reason,
        string ReceiverName,
        string ReceiverPhone,
        string ReceiverEmail,
        MedicineBatchReturnStatus Status,
        DateTime? ApprovedAt,
        DateTime? RejectedAt,
        DateTime? CreatedAt,
        List<MedicineBatchReturnDetailItemDto> Details
    );

    public record MedicineBatchReturnDetailItemDto(
        int Id,
        int MedicineBatchId,
        string BatchNumber,
        DateOnly ExpirationDate,
        decimal Quantity
    );
}
