namespace Inventory.Application.Medicines.Queries.GetMedicineInteractionByMedicineId
{
    public class GetMedicineInteractionsByMedicineIdQueryHandler(IApplicationDbContext dbContext)
        : IQueryHandler<GetMedicineInteractionsByMedicineIdQuery, GetMedicineInteractionsByMedicineIdResult>
    {
        public async Task<GetMedicineInteractionsByMedicineIdResult> Handle(GetMedicineInteractionsByMedicineIdQuery request, CancellationToken cancellationToken)
        {
            var medicineExists = await dbContext.Medicines
                .AnyAsync(m => m.Id == request.MedicineId, cancellationToken);

            if (!medicineExists)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);

            var interactions = await dbContext.MedicineInteractions
                .Where(mi => mi.MedicineId1 == request.MedicineId || mi.MedicineId2 == request.MedicineId)
                .Where(x => x.IsCancelled == false)
                .OrderBy(mi => mi.Id)
                .ToListAsync(cancellationToken);

            var interactionsDTO = interactions.Adapt<List<MedicineInteractionDTO>>();

            return new GetMedicineInteractionsByMedicineIdResult(interactionsDTO);
        }
    }
}
