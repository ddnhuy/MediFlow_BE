namespace Inventory.Application.Medicines.Commands.UpdateMedicinePrice
{
    public class UpdateMedicinePriceCommandHandler(IApplicationDbContext dbContext)
        : ICommandHandler<UpdateMedicinePriceCommand, UpdateMedicinePriceResult>
    {
        public async Task<UpdateMedicinePriceResult> Handle(
            UpdateMedicinePriceCommand request, CancellationToken cancellationToken)
        {
            // Check if medicine exists
            var medicine = await dbContext.Medicines
                .FirstOrDefaultAsync(m => m.Id == request.MedicineId && !m.IsSuspended && !m.IsCancelled, cancellationToken);

            if (medicine == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            // Find existing medicine price by ID
            var medicinePrice = await dbContext.MedicinePrices
                .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsCancelled, cancellationToken);

            if (medicinePrice == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_PRICE);
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
