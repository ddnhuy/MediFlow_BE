namespace Inventory.Application.Medicines.Queries.GetMedicinePricesByMedicineId
{
    public class GetMedicinePricesByMedicineIdQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicinePricesByMedicineIdQuery, GetMedicinePricesByMedicineIdResult>
    {
        public async Task<GetMedicinePricesByMedicineIdResult> Handle(
            GetMedicinePricesByMedicineIdQuery query, CancellationToken cancellationToken)
        {
            var meidicine = await dbContext.Medicines
                .FirstOrDefaultAsync(m => m.Id == query.MedicineId && !m.IsSuspended && !m.IsCancelled, cancellationToken);

            if (meidicine == null)
            {
                throw new MedicineNotFoundException(InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_ID(query.MedicineId));
            }

            var medicinePrices = await dbContext.MedicinePrices
                .FirstOrDefaultAsync(p => p.MedicineId == query.MedicineId && !p.IsSuspended && !p.IsCancelled, cancellationToken);

            if (medicinePrices == null)
            {
                throw new NotFoundException("Không tìm thấy giá tiền của thuốc trên");
            }

            var medicinePriceDTOs = medicinePrices.Adapt<MedicinePriceDTO>();

            return new GetMedicinePricesByMedicineIdResult(medicinePriceDTOs);
        }
    }
}
