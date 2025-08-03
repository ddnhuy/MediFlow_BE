using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Commands.RejectMedicineBatchReturn
{
    public class RejectMedicineBatchReturnCommandHandler(IApplicationDbContext dbContext)
        : ICommandHandler<RejectMedicineBatchReturnCommand, RejectMedicineBatchReturnResult>
    {
        public async Task<RejectMedicineBatchReturnResult> Handle(RejectMedicineBatchReturnCommand request, CancellationToken cancellationToken)
        {
            var medicineBatchReturn = await dbContext.MedicineBatchReturns
                .FirstOrDefaultAsync(mbr => mbr.Id == request.Id, cancellationToken);

            if (medicineBatchReturn == null)
            {
                throw new NotFoundException(ExceptionKey.MEDICINE_BATCH_RETURN_NOT_FOUND);
            }

            if (medicineBatchReturn.Status != MedicineBatchReturnStatus.Pending)
            {
                throw new BadRequestException(ExceptionKey.MEDICINE_BATCH_RETURN_ALREADY_PROCESSED);
            }

            if (medicineBatchReturn.ApprovalToken != request.Token)
            {
                throw new BadRequestException(ExceptionKey.INVALID_APPROVAL_TOKEN);
            }

            // Update return status
            medicineBatchReturn.Status = MedicineBatchReturnStatus.Rejected;
            medicineBatchReturn.RejectedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new RejectMedicineBatchReturnResult(true);
        }
    }
}