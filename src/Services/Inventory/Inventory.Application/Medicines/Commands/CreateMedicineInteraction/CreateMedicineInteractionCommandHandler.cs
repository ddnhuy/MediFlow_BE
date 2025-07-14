namespace Inventory.Application.Medicines.Commands.CreateMedicineInteraction
{
    public class CreateMedicineInteractionCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateMedicineInteractionCommand, CreateMedicineInteractionResult>
    {
        public async Task<CreateMedicineInteractionResult> Handle(CreateMedicineInteractionCommand request, CancellationToken cancellationToken)
        {
            // Verify both medicines exist
            var medicine1Exists = await dbContext.Medicines.AnyAsync(m => m.Id == request.MedicineId1, cancellationToken);
            var medicine2Exists = await dbContext.Medicines.AnyAsync(m => m.Id == request.MedicineId2, cancellationToken);

            if (!medicine1Exists)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);

            if (!medicine2Exists)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);

            // Check if interaction already exists
            var existingInteraction = await dbContext.MedicineInteractions
                .AnyAsync(mi =>
                    mi.MedicineId1 == request.MedicineId1 && mi.MedicineId2 == request.MedicineId2 ||
                    mi.MedicineId1 == request.MedicineId2 && mi.MedicineId2 == request.MedicineId1,
                    cancellationToken);

            if (existingInteraction)
                throw new BadRequestException(ExceptionKey.INTERACTION_ALREADY_EXISTS);

            var interaction = new MedicineInteraction
            {
                MedicineId1 = request.MedicineId1,
                MedicineId2 = request.MedicineId2,
                HarmfulEffects = request.HarmfulEffects,
                Mechanism = request.Mechanism,
                PreventiveActions = request.PreventiveActions,
                ReferenceInfo = request.ReferenceInfo,
                Notes = request.Notes,
                IsSuspended = false,
                IsCancelled = false,
            };

            await dbContext.MedicineInteractions.AddAsync(interaction, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new CreateMedicineInteractionResult(interaction.Id);
        }
    }
}
