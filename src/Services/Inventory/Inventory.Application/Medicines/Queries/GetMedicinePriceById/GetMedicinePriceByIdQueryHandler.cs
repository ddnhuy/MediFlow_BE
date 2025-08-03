namespace Inventory.Application.Medicines.Queries.GetMedicinePriceById
{
    public class GetMedicinePriceByIdQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicinePriceByIdQuery, GetMedicinePriceByIdResult>
    {
        public async Task<GetMedicinePriceByIdResult> Handle(GetMedicinePriceByIdQuery query, CancellationToken cancellationToken)
        {
            var medicinePrice = await dbContext.MedicinePrices
                .Where(p => p.Id == query.Id && !p.IsCancelled)
                .Include(p => p.Medicine)
                .FirstOrDefaultAsync(cancellationToken);

            if (medicinePrice == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_PRICE);
            }

            var medicinePriceDTO = new MedicinePriceDTO
            {
                MedicineId = medicinePrice.MedicineId,
                Id = medicinePrice.Id,
                MedicineName = medicinePrice.Medicine?.MedicineName ?? string.Empty,
                Currency = medicinePrice.Currency,
                UnitPrice = medicinePrice.UnitPrice,
                VatAmount = medicinePrice.VatAmount,
                OriginalPriceAfterVat = medicinePrice.OriginalPriceAfterVat,
                LastUpdatedBy = medicinePrice.LastUpdatedBy,
                VatRate = medicinePrice.VatRate,
                IsSuspended = medicinePrice.IsSuspended,
                OriginalPriceBeforeVat = medicinePrice.OriginalPriceBeforeVat,
                CreatedAt = medicinePrice.CreatedAt,
                IsCancelled = medicinePrice.IsCancelled,
                LastUpdatedAt = medicinePrice.LastUpdatedAt,
                CreatedBy = medicinePrice.CreatedBy,
            };

            return new GetMedicinePriceByIdResult(medicinePriceDTO);
        }
    }
}