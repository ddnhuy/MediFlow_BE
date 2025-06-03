namespace Inventory.Application.Medicines.Commands.DeleteMedicinePrice
{
    public class DeleteMedicinePriceCommandHandler(IApplicationDbContext dbContext)
        : ICommandHandler<DeleteMedicinePriceCommand, DeleteMedicinePriceResult>
    {
        public async Task<DeleteMedicinePriceResult> Handle(
            DeleteMedicinePriceCommand request, CancellationToken cancellationToken)
        {
            var medicinePrice = await dbContext.MedicinePrices
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsSuspended && !p.IsCancelled, cancellationToken);

            if (medicinePrice == null)
            {
                throw new NotFoundException($"Không tìm thấy giá thuốc với ID {request.Id}");
            }

            // Soft delete by marking as cancelled
            medicinePrice.IsSuspended = true;
            medicinePrice.IsCancelled = true;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteMedicinePriceResult(true);
        }
    }
}
