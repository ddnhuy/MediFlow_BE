using BuildingBlocks.Strings.ExceptionStrings;

namespace Inventory.Application.Medicines.Commands.DeleteMedicine
{
    public class DeleteMedicineCommandHandler : ICommandHandler<DeleteMedicineCommand, DeleteMedicineResult>
    {
        private readonly IApplicationDbContext _dbContext;

        public DeleteMedicineCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeleteMedicineResult> Handle(DeleteMedicineCommand request, CancellationToken cancellationToken)
        {
            var medicine = await _dbContext.Medicines.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (medicine == null)
            {
                throw new MedicineNotFoundException(InventoryExceptionStrings.NOT_FOUND_MEDICINE_WITH_ID(request.Id));
            }

            medicine.IsSuspended = true;
            medicine.IsCancelled = true;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteMedicineResult(true);
        }
    }
}
