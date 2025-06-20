namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateSupplierCommand, CreateSupplierResult>
    {
        public async Task<CreateSupplierResult> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            bool duplicateCodeExists = await dbContext.Suppliers
                .AnyAsync(x => x.SupplierCode == request.SupplierCode, cancellationToken);

            if (duplicateCodeExists)
            {
                throw new BadRequestException(ExceptionKey.DUPLICATE_SUPPLIER_CODE);
            }

            var supplier = new Supplier
            {
                SupplierCode = request.SupplierCode,
                SupplierName = request.SupplierName,
                Address = request.Address,
                Phone = request.Phone,
                Fax = request.Fax,
                Email = request.Email,
                TaxCode = request.TaxCode,
                Director = request.Director,
                ContactPerson = request.ContactPerson,
                IsCancelled = false,
                IsSuspended = false,
            };

            var result = dbContext.Suppliers.Add(supplier);
            await dbContext.SaveChangesAsync(cancellationToken);

            if (result.Entity.Id < 0)
            {
                throw new BadRequestException(ExceptionKey.FAILED_CREATE_SUPPLIER_WITH_ID);
            }

            return new CreateSupplierResult(result.Entity.Id);
        }
    }
}
