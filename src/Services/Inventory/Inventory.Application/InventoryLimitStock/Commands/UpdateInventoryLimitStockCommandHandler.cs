using FluentValidation;

namespace Inventory.Application.InventoryLimitStock.Commands
{
    public record UpdateInventoryLimitStockCommand(int Id, int MedicineId, decimal MinimalStockThreshold) : ICommand<UpdateInventoryLimitStockResult>;

    public record UpdateInventoryLimitStockResult(bool IsSuccess);

    public class UpdateInventoryLimitStockCommandValidator : AbstractValidator<UpdateInventoryLimitStockCommand>
    {
        public UpdateInventoryLimitStockCommandValidator()
        {
            RuleFor(x => x.MinimalStockThreshold).GreaterThanOrEqualTo(0).WithMessage(ExceptionKey.MINIMUM_STOCK_THRESHOLD_MUST_BE_POSITIVE.ToString());
            RuleFor(x => x.Id).GreaterThan(0).WithMessage(ExceptionKey.NOT_FOUND_INVENTORY_LIMIT_STOCK_WITH_ID.ToString());           
        }
    }

    public class UpdateInventoryLimitStockCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateInventoryLimitStockCommand, UpdateInventoryLimitStockResult>
    {
        public async Task<UpdateInventoryLimitStockResult> Handle(UpdateInventoryLimitStockCommand request, CancellationToken cancellationToken)
        {
            // Validate the existence of the inventory limit stock
            var inventoryLimitStock = await dbContext.InventoryLimitStocks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (inventoryLimitStock == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_INVENTORY_LIMIT_STOCK_WITH_ID);
            }

            // Validate the existence of the medicine
            var medicine = await dbContext.Medicines
                .FirstOrDefaultAsync(x => x.Id == request.MedicineId, cancellationToken);
            if (medicine == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            // Check if the medicine is already associated with another inventory limit stock
            var existingInventoryLimitStock = await dbContext.InventoryLimitStocks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MedicineId == request.MedicineId && x.Id != request.Id, cancellationToken);
            if (existingInventoryLimitStock != null)
            {
                throw new BadRequestException(ExceptionKey.INVENTORY_LIMIT_STOCK_ALREADY_EXISTS_FOR_MEDICINE);
            }

            // Update the inventory limit stock
            inventoryLimitStock.MedicineId = request.MedicineId;
            inventoryLimitStock.MinimalStockThreshold = request.MinimalStockThreshold;

            dbContext.InventoryLimitStocks.Update(inventoryLimitStock);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateInventoryLimitStockResult(true);
        }
    }
}