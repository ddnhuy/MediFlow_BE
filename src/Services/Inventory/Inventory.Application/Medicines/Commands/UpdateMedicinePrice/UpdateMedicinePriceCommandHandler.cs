namespace Inventory.Application.Medicines.Commands.UpdateMedicinePrice
{
    public class UpdateMedicinePriceCommandHandler(IApplicationDbContext dbContext)
        : ICommandHandler<UpdateMedicinePriceCommand, UpdateMedicinePriceResult>
    {
        public async Task<UpdateMedicinePriceResult> Handle(
            UpdateMedicinePriceCommand request, CancellationToken cancellationToken)
        {
            var medicine = await dbContext.Medicines
                .FirstOrDefaultAsync(m => m.Id == request.MedicineId && !m.IsSuspended && !m.IsCancelled, cancellationToken);

            if (medicine == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            // Check if medicine price exists
            var medicinePrice = await dbContext.MedicinePrices
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, cancellationToken);

            if (medicinePrice == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_PRICE);
            }

            if (medicinePrice.MedicineId != request.MedicineId)
            {
                throw new BadRequestException(ExceptionKey.MEDICINE_ALREADY_HAVE_PRICE);
            }

            // Update medicine price properties
            medicinePrice.MedicineId = request.MedicineId;
            medicinePrice.UnitPrice = request.UnitPrice;
            medicinePrice.Currency = request.Currency;
            medicinePrice.VatRate = request.VatRate;
            medicinePrice.VatAmount = request.VatAmount;
            medicinePrice.OriginalPriceAfterVat = request.OriginalPriceAfterVat;
            medicinePrice.OriginalPriceBeforeVat = request.OriginalPriceBeforeVat;
            medicinePrice.IsSuspended = request.IsSuspended;
            medicinePrice.IsCancelled = request.IsCancelled;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateMedicinePriceResult(true);
        }
    }
}
