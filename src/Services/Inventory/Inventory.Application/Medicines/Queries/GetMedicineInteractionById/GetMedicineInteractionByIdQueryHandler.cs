
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Inventory.Application.Medicines.Queries.GetMedicineInteractionById
{
    public class GetMedicineInteractionByIdQueryHandler : IQueryHandler<GetMedicineInteractionByIdQuery, GetMedicineInteractionByIdResponse>
    {
        private readonly IApplicationDbContext _dbContext;

        public GetMedicineInteractionByIdQueryHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetMedicineInteractionByIdResponse> Handle(GetMedicineInteractionByIdQuery request, CancellationToken cancellationToken)
        {
            var interaction = await _dbContext.MedicineInteractions
            .Include(x => x.Medicine1)
            .Include(x => x.Medicine2)
            .Where(x => x.Id == request.Id && !x.IsCancelled)
            .Select(x => new MedicineInteractionDTO
            {
                Id = x.Id,
                MedicineName1 = x.Medicine1.MedicineName,
                MedicineId1 = x.MedicineId1,
                MedicineName2 = x.Medicine2.MedicineName,
                MedicineId2 = x.MedicineId2,
                Mechanism = x.Mechanism,
                ReferenceInfo = x.ReferenceInfo,
                HarmfulEffects = x.HarmfulEffects,
                IsSuspended = x.IsSuspended,
                Notes = x.Notes,
                PreventiveActions = x.PreventiveActions,
                CreatedAt = x.CreatedAt,
                IsCancelled = x.IsCancelled,
                LastUpdatedAt = x.LastUpdatedAt,
                CreatedBy = x.CreatedBy,
                LastUpdatedBy = x.LastUpdatedBy
            })
            .FirstOrDefaultAsync(cancellationToken);

            if (interaction == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_INTERACTION_WITH_ID);
            }

            return new GetMedicineInteractionByIdResponse(interaction);
        }
    }
}
