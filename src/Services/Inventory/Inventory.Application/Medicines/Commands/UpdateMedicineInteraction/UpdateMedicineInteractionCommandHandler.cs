namespace Inventory.Application.Medicines.Commands.UpdateMedicineInteraction
{
    public class UpdateMedicineInteractionCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateMedicineInteractionCommand, UpdateMedicineInteractionResult>
    {
        public async Task<UpdateMedicineInteractionResult> Handle(UpdateMedicineInteractionCommand request, CancellationToken cancellationToken)
        {
            var interaction = await dbContext.MedicineInteractions.FindAsync(new object[] { request.Id }, cancellationToken);

            if (interaction == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_INTERACTION_WITH_ID);
            }

            // Verify both medicines exist
            var medicine1Exists = await dbContext.Medicines.AnyAsync(m => m.Id == request.MedicineId1 && !m.IsSuspended, cancellationToken);
            var medicine2Exists = await dbContext.Medicines.AnyAsync(m => m.Id == request.MedicineId2 && !m.IsSuspended, cancellationToken);

            if (!medicine1Exists)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);

            if (!medicine2Exists)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);

            // Check if a different interaction with the same medicines exists
            var existingInteraction = await dbContext.MedicineInteractions
                .AnyAsync(mi =>
                    mi.Id != request.Id &&
                    (mi.MedicineId1 == request.MedicineId1 && mi.MedicineId2 == request.MedicineId2 ||
                     mi.MedicineId1 == request.MedicineId2 && mi.MedicineId2 == request.MedicineId1),
                    cancellationToken);

            if (existingInteraction)
                throw new BadRequestException(ExceptionKey.INTERACTION_ALREADY_EXISTS);

            interaction.MedicineId1 = request.MedicineId1;
            interaction.MedicineId2 = request.MedicineId2;
            interaction.HarmfulEffects = request.HarmfulEffects;
            interaction.Mechanism = request.Mechanism;
            interaction.PreventiveActions = request.PreventiveActions;
            interaction.ReferenceInfo = request.ReferenceInfo;
            interaction.Notes = request.Notes;
            interaction.IsSuspended = request.IsSuspended;
            interaction.IsCancelled = request.IsCancelled;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateMedicineInteractionResult(true);
        }
    }
}
