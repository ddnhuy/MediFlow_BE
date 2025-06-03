namespace Inventory.Application.Medicines.Commands.CreateMedicinePrice
{
    public class CreateMedicinePriceCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateMedicinePriceCommand, CreateMedicinePriceResult>
    {
        public async Task<CreateMedicinePriceResult> Handle(
            CreateMedicinePriceCommand request, CancellationToken cancellationToken)
        {
            // Check if medicine exists
            var medicine = await dbContext.Medicines
                .FirstOrDefaultAsync(m => m.Id == request.MedicineId && !m.IsSuspended && !m.IsCancelled, cancellationToken);

            if (medicine == null)
            {
                throw new MedicineNotFoundException(InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_ID(request.MedicineId));
            }

            // Create new medicine price
            var medicinePrice = new MedicinePrice
            {
                MedicineId = request.MedicineId,
                UnitPrice = request.UnitPrice,
                Currency = request.Currency,
                VatRate = request.VatRate,
                VatAmount = request.VatAmount,
                OriginalPriceAfterVat = request.OriginalPriceAfterVat,
                OriginalPriceBeforeVat = request.OriginalPriceBeforeVat,
            };

            await dbContext.MedicinePrices.AddAsync(medicinePrice, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateMedicinePriceResult(medicinePrice.Id);
        }
    }
}
