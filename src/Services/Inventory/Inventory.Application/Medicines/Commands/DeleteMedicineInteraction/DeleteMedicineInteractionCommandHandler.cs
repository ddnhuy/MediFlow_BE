using BuildingBlocks.Strings.ExceptionStrings;

namespace Inventory.Application.Medicines.Commands.DeleteMedicineInteraction
{
    public class DeleteMedicineInteractionCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteMedicineInteractionCommand, DeleteMedicineInteractionResult>
    {
        public async Task<DeleteMedicineInteractionResult> Handle(DeleteMedicineInteractionCommand request, CancellationToken cancellationToken)
        {
            var interaction = await dbContext.MedicineInteractions.FirstOrDefaultAsync(x => x.Id == request.Id , cancellationToken);

            if (interaction == null)
            {
                throw new NotFoundException(InventoryExceptionStrings.NOT_FOUND_INTERACTION_WITH_ID(request.Id));
            }

            // Soft delete
            interaction.IsSuspended = true;
            interaction.IsCancelled = true;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteMedicineInteractionResult(true);
        }
    }
}
