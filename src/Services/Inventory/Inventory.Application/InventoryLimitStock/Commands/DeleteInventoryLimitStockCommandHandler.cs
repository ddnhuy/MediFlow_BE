namespace Inventory.Application.InventoryLimitStock.Commands
{
    public record DeleteInventoryLimitStockCommand(int Id) : ICommand<DeleteInventoryLimitStockResult>;
    public record DeleteInventoryLimitStockResult(bool IsSuccess);

    public class DeleteInventoryLimitStockCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteInventoryLimitStockCommand, DeleteInventoryLimitStockResult>
    {
        public async Task<DeleteInventoryLimitStockResult> Handle(DeleteInventoryLimitStockCommand request, CancellationToken cancellationToken)
        {
            var entity = await dbContext.InventoryLimitStocks
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsSuspended && !x.IsCancelled, cancellationToken);

            if (entity == null)
                throw new NotFoundException(ExceptionKey.NOT_FOUND_INVENTORY_LIMIT_STOCK_WITH_ID);

            // Soft delete
            entity.IsCancelled = true;
            entity.IsSuspended = true;

            dbContext.InventoryLimitStocks.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteInventoryLimitStockResult(true);
        }
    }
}