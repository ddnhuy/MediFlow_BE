using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Queries.GetMedicineBatchReturnById
{
    public class GetMedicineBatchReturnByIdQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicineBatchReturnByIdQuery, GetMedicineBatchReturnByIdResult>
    {
        public async Task<GetMedicineBatchReturnByIdResult> Handle(GetMedicineBatchReturnByIdQuery request, CancellationToken cancellationToken)
        {
            // Get the medicine batch return
            var medicineBatchReturn = await dbContext.MedicineBatchReturns
                .AsNoTracking()
                .FirstOrDefaultAsync(mbr => mbr.Id == request.Id, cancellationToken);

            if (medicineBatchReturn == null)
            {
                throw new BadRequestException(ExceptionKey.MEDICINE_BATCH_RETURN_NOT_FOUND);
            }

            // Get the details for this return
            var details = await dbContext.MedicineBatchReturnDetails
                .Where(d => d.MedicineBatchReturnId == request.Id)
                .AsNoTracking()
                .Select(detail => new MedicineBatchReturnDetailItemDto(
                    detail.Id,
                    detail.MedicineBatchId,
                    detail.BatchNumber ?? string.Empty,
                    detail.ExpirationDate,
                    detail.Quantity
                ))
                .ToListAsync(cancellationToken);

            var result = new MedicineBatchReturnDetailDto(
                Id: medicineBatchReturn.Id,
                ReturnCode: medicineBatchReturn.ReturnCode,
                Reason: medicineBatchReturn.Reason,
                ReceiverName: medicineBatchReturn.ReceiverName,
                ReceiverPhone: medicineBatchReturn.ReceiverPhone,
                ReceiverEmail: medicineBatchReturn.ReceiverEmail,
                Status: medicineBatchReturn.Status,
                ApprovedAt: medicineBatchReturn.ApprovedAt,
                RejectedAt: medicineBatchReturn.RejectedAt,
                CreatedAt: medicineBatchReturn.CreatedAt,
                Details: details
            );

            return new GetMedicineBatchReturnByIdResult(result);
        }
    }
}