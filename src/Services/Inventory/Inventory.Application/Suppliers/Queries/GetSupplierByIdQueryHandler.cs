namespace Inventory.Application.Suppliers.Queries.GetSupplierById
{
    public class GetSupplierByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetSupplierByIdQuery, GetSupplierByIdResult>
    {
        public async Task<GetSupplierByIdResult> Handle(GetSupplierByIdQuery request, CancellationToken cancellationToken)
        {
            var supplier = await dbContext.Suppliers
                .Where(x => x.Id == request.Id && !x.IsCancelled)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (supplier == null)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_SUPPLIER_WITH_ID);
            }

            var contracts = await dbContext.SupplierContracts
                .Where(c => c.SupplierId == request.Id && !c.IsCancelled)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var supplierDetailDTO = new SupplierDetailDTO
            {
                Id = supplier.Id,
                SupplierCode = supplier.SupplierCode,
                SupplierName = supplier.SupplierName,
                Address = supplier.Address,
                Phone = supplier.Phone,
                Fax = supplier.Fax,
                Email = supplier.Email,
                TaxCode = supplier.TaxCode,
                Director = supplier.Director,
                ContactPerson = supplier.ContactPerson,
                ExpiredDate = supplier.ExpiredDate,
                IsSuspended = supplier.IsSuspended,
                IsCancelled = supplier.IsCancelled,
                CreatedAt = supplier.CreatedAt,
                CreatedBy = supplier.CreatedBy,
                LastUpdatedAt = supplier.LastUpdatedAt,
                LastUpdatedBy = supplier.LastUpdatedBy,
                Contracts = contracts.Select(c => new SupplierContractDTO
                {
                    Id = c.Id,
                    FileName = c.FileName
                }).ToList()
            };

            return new GetSupplierByIdResult(supplierDetailDTO);
        }
    }
}
