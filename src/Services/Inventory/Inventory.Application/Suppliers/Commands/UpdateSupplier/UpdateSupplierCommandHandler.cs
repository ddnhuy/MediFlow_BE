namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateSupplierCommand, UpdateSupplierResult>
    {
        public async Task<UpdateSupplierResult> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await dbContext.Suppliers.FindAsync(request.Id);

            if (supplier == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_SUPPLIER_WITH_ID);
            }

            bool duplicateCodeExists = await dbContext.Suppliers
                .AnyAsync(x => x.SupplierCode == request.SupplierCode && x.Id != request.Id, cancellationToken);

            if (duplicateCodeExists)
            {
                throw new InvalidOperationException(ExceptionKey.DUPLICATE_SUPPLIER_CODE.ToString());
            }

            supplier.SupplierCode = request.SupplierCode;
            supplier.SupplierName = request.SupplierName;
            supplier.Phone = request.Phone;
            supplier.Fax = request.Fax;
            supplier.Email = request.Email;
            supplier.TaxCode = request.TaxCode;
            supplier.Address = request.Address;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Director = request.Director;
            supplier.IsSuspended = request.IsSuspended;
            supplier.IsCancelled = request.IsCancelled;

            var result = await dbContext.SaveChangesAsync(cancellationToken);

            if (result < 0)
            {
                throw new InvalidOperationException(ExceptionKey.FAILED_UPDATE_SUPPLIER_WITH_ID.ToString());
            }

            return new UpdateSupplierResult(true);
        }
    }
}
