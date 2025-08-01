using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Commands.ApproveMedicineBatchReturn
{
    public class ApproveMedicineBatchReturnCommandHandler(IApplicationDbContext dbContext)
        : ICommandHandler<ApproveMedicineBatchReturnCommand, ApproveMedicineBatchReturnResult>
    {
        public async Task<ApproveMedicineBatchReturnResult> Handle(ApproveMedicineBatchReturnCommand request, CancellationToken cancellationToken)
        {
            var medicineBatchReturn = await dbContext.MedicineBatchReturns
                .FirstOrDefaultAsync(mbr => mbr.Id == request.Id, cancellationToken);

            var medicineBatchReturnDetails = await dbContext.MedicineBatchReturnDetails
                .Where(mbrd => mbrd.MedicineBatchReturnId == request.Id)
                .ToListAsync(cancellationToken);

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
            medicineBatchReturn.Status = MedicineBatchReturnStatus.Approved;
            medicineBatchReturn.ApprovedAt = DateTime.UtcNow;

            // Update batch statuses
            var batchIds = medicineBatchReturnDetails.Select(d => d.MedicineBatchId).ToList();
            var batches = await dbContext.MedicineBatches
                .Where(mb => batchIds.Contains(mb.Id))
                .ToListAsync(cancellationToken);

            foreach (var batch in batches)
            {
                batch.Status = MedicineBatchStatus.IsReturned;
            }

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ApproveMedicineBatchReturnResult(true);
        }
    }
}
