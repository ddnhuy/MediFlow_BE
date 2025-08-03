namespace Inventory.Application.Suppliers.Commands.DeleteSupplier
{
    public class DeleteSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteSupplierCommand, DeleteSupplierResult>
    {
        public async Task<DeleteSupplierResult> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await dbContext.Suppliers.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsCancelled);

            if (supplier == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_SUPPLIER_WITH_ID);
            }

            supplier.IsCancelled = true;
            supplier.IsSuspended = true;

            int result = await dbContext.SaveChangesAsync(cancellationToken);
            if (result < 0)
            {
                return new DeleteSupplierResult(false);
            }

            var supplierContracts = await dbContext.SupplierContracts
                .Where(x => x.SupplierId == request.Id && !x.IsCancelled)
                .ToListAsync(cancellationToken);

            foreach (var contract in supplierContracts)
            {
                // If soft-deleted (marked as IsCancelled = true), it still exists in the database with the same PK,
                // so when try to re-add a contract with the same ID, it will cause a primary key constraint violation.
                dbContext.SupplierContracts.Remove(contract);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return new DeleteSupplierResult(true);
        }
    }
}
