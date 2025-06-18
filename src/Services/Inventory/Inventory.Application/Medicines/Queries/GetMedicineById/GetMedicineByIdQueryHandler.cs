namespace Inventory.Application.Medicines.Queries.GetMedicineById
{
    public class GetMedicineByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineByIdQuery, GetMedicineByIdResult>
    {
        public async Task<GetMedicineByIdResult> Handle(GetMedicineByIdQuery request, CancellationToken cancellationToken)
        {
            var medicine = await dbContext.Medicines.Where(x => !x.IsSuspended)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id);

            if (medicine == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            var medicineDTO = medicine.Adapt<MedicineDTO>();

            return new GetMedicineByIdResult(medicineDTO);
        }
    }
}
