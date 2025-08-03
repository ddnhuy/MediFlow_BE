namespace Inventory.Application.InventoryLimitStock.Commands
{
    public record CreateInventoryLimitStockCommand(int MedicineId, decimal MinimalStockThreshold) : ICommand<CreateInventoryLimitStockResult>;

    public record CreateInventoryLimitStockResult(bool IsSuccess, int Id);

    public class CreateInventoryLimitStockCommandValidator : AbstractValidator<CreateInventoryLimitStockCommand>
    {
        public CreateInventoryLimitStockCommandValidator()
        {
            RuleFor(x => x.MedicineId).GreaterThan(0).WithMessage(ExceptionKey.REQUIRED_MEDICINE_ID.ToString());
            RuleFor(x => x.MinimalStockThreshold).GreaterThan(0).WithMessage(ExceptionKey.MINIMUM_STOCK_THRESHOLD_MUST_BE_POSITIVE.ToString());
        }
    }

    public class CreateInventoryLimitStockCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateInventoryLimitStockCommand, CreateInventoryLimitStockResult>
    {
        public async Task<CreateInventoryLimitStockResult> Handle(CreateInventoryLimitStockCommand request, CancellationToken cancellationToken)
        {

            var medicine = await dbContext.Medicines.FirstOrDefaultAsync(x => x.Id == request.MedicineId && !x.IsSuspended && !x.IsCancelled, cancellationToken);

            if (medicine == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            var existingInventoryLimitStock = await dbContext.InventoryLimitStocks
                .FirstOrDefaultAsync(x => x.MedicineId == request.MedicineId && !x.IsCancelled, cancellationToken);
            if (existingInventoryLimitStock != null)
            {
                throw new BadRequestException(ExceptionKey.INVENTORY_LIMIT_STOCK_ALREADY_EXISTS_FOR_MEDICINE);
            }

            var inventoryLimitStock = new Inventory.Domain.Models.InventoryLimitStock
            {
                MedicineId = request.MedicineId,
                MinimalStockThreshold = request.MinimalStockThreshold
            };

            await dbContext.InventoryLimitStocks.AddAsync(inventoryLimitStock, cancellationToken);
            var result = await dbContext.SaveChangesAsync(cancellationToken);        

            return new CreateInventoryLimitStockResult(true, inventoryLimitStock.Id);

        }
    }

}
