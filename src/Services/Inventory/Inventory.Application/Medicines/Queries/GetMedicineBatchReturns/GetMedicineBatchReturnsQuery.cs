using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Queries.GetMedicineBatchReturns
{
    public record GetMedicineBatchReturnsQuery(
        PaginationRequest Pagination,
        string? SearchReturnCode = null,
        MedicineBatchReturnStatus? Status = null
    ) : IQuery<GetMedicineBatchReturnsResult>;

    public record GetMedicineBatchReturnsResult(PaginatedResult<MedicineBatchReturnDto> MedicineBatchReturns);

    public record MedicineBatchReturnDto(
        int Id,
        string ReturnCode,
        string ReceiverName,
        string ReceiverPhone,
        string ReceiverEmail,
        MedicineBatchReturnStatus Status,
        DateTime CreatedAt
    );
}
