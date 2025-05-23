namespace Inventory.Application.Medicines.Queries.GetMedicineInteraction
{
    public class GetMedicineInteractionsQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicineInteractionsQuery, GetMedicineInteractionsResult>
    {
        public async Task<GetMedicineInteractionsResult> Handle(GetMedicineInteractionsQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCount = await dbContext.MedicineInteractions.Where(x => !x.IsSuspended).LongCountAsync(cancellationToken: cancellationToken);

            var interactions = await dbContext.MedicineInteractions
                .Where(x => !x.IsSuspended)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            var interactionDTOs = interactions.Adapt<List<MedicineInteractionDTO>>();

            return new GetMedicineInteractionsResult(new PaginatedResult<MedicineInteractionDTO>(pageIndex, pageSize, totalCount, interactionDTOs));
        }
    }
}
