
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
            .Where(x => x.Id == request.Id && !x.IsSuspended)
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
            .FirstOrDefaultAsync(cancellationToken);

            if (interaction == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_INTERACTION_WITH_ID);
            }

            return new GetMedicineInteractionByIdResponse(interaction);
        }
    }
}
