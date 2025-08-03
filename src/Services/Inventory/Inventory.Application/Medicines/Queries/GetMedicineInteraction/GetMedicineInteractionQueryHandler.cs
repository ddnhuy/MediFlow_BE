namespace Inventory.Application.Medicines.Queries.GetMedicineInteraction
{
    public class GetMedicineInteractionsQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineInteractionsQuery, GetMedicineInteractionsResult>
    {
        public async Task<GetMedicineInteractionsResult> Handle(GetMedicineInteractionsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await dbContext.MedicineInteractions.Where(x => !x.IsCancelled).LongCountAsync(cancellationToken: cancellationToken);

            var interactions = await dbContext.MedicineInteractions
            .Include(x => x.Medicine1)
            .Include(x => x.Medicine2)
            .Where(x => !x.IsCancelled)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new MedicineInteractionDTO
            {
                Id = x.Id,
                MedicineId1 = x.MedicineId1,
                MedicineName1 = x.Medicine1.MedicineName,
                MedicineId2 = x.MedicineId2,
                MedicineName2 = x.Medicine2.MedicineName,
                HarmfulEffects = x.HarmfulEffects,
                Mechanism = x.Mechanism,
                PreventiveActions = x.PreventiveActions,
                ReferenceInfo = x.ReferenceInfo,
                Notes = x.Notes,
                IsSuspended = x.IsSuspended,
                IsCancelled = x.IsCancelled,
                CreatedAt = x.CreatedAt,
                CreatedBy = x.CreatedBy,
                LastUpdatedAt = x.LastUpdatedAt,
                LastUpdatedBy = x.LastUpdatedBy
            })
            .ToListAsync(cancellationToken: cancellationToken);

            return new GetMedicineInteractionsResult(new PaginatedResult<MedicineInteractionDTO>(pageIndex, pageSize, totalCount, interactions));
        }
    }
}
