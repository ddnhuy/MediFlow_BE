namespace Inventory.Application.Medicines.Queries.GetMedicines
{
    public class GetMedicineQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetMedicinesQuery, GetMedicinesResult>
    {
        public async Task<GetMedicinesResult> Handle(GetMedicinesQuery query, CancellationToken cancellationToken)
        {
            var pageIndex = query.PaginationRequest.PageIndex;
            var pageSize = query.PaginationRequest.PageSize;

            var totalCounts = await dbContext.Medicines.Where(x => !x.IsSuspended).LongCountAsync(cancellationToken: cancellationToken);

            var medicines = await dbContext.Medicines
                .Where(x => !x.IsSuspended)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);

            var medicineDTOs = medicines.Adapt<List<MedicineDTO>>();

            return new GetMedicinesResult(new PaginatedResult<MedicineDTO>(pageIndex, pageSize, totalCounts, medicineDTOs));
        }
    }
}
