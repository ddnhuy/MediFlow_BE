namespace Inventory.Application.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateSupplierCommand, UpdateSupplierResult>
    {
        public async Task<UpdateSupplierResult> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplier = await dbContext.Suppliers
                .FirstOrDefaultAsync(s => s.Id == request.Id);

            if (supplier == null)
            {
                throw new NotFoundException(ExceptionKey.NOT_FOUND_SUPPLIER_WITH_ID);
            }

            // Check for contracts that already belong to another supplier
            if (request.Contracts != null && request.Contracts.Any())
            {
                // Get all contract IDs in the request
                var contractIds = request.Contracts.Select(c => c.Id).ToList();

                // Find contracts that are already assigned to a different supplier and not cancelled
                var conflictingContracts = await dbContext.SupplierContracts
                    .Where(sc => contractIds.Contains(sc.Id)
                                 && sc.SupplierId != request.Id
                                 && !sc.IsCancelled)
                    .ToListAsync(cancellationToken);

                if (conflictingContracts.Any())
                {
                    throw new BadRequestException(
                        ExceptionKey.CONTRACT_IS_ALREADY_BELONG_TO_ANOTHER_SUPPLIER
                    );
                }
            }

            // Update supplier properties
            supplier.SupplierName = request.SupplierName;
            supplier.Phone = request.Phone;
            supplier.Fax = request.Fax;
            supplier.Email = request.Email;
            supplier.TaxCode = request.TaxCode;
            supplier.Address = request.Address;
            supplier.ContactPerson = request.ContactPerson;
            supplier.Director = request.Director;
            supplier.ExpiredDate = request.ExpiredDate;
            supplier.IsSuspended = request.IsSuspended;
            supplier.IsCancelled = request.IsCancelled;

            // Handle contracts replacement
            if (request.Contracts != null)
            {
                await ReplaceSupplierContractsAsync(supplier.Id, request.Contracts, cancellationToken);
            }

            var result = await dbContext.SaveChangesAsync(cancellationToken);

            if (result < 0)
            {
                throw new InvalidOperationException(ExceptionKey.FAILED_UPDATE_SUPPLIER_WITH_ID.ToString());
            }

            return new UpdateSupplierResult(true);
        }

        private async Task ReplaceSupplierContractsAsync(int supplierId, List<UpdateSupplierContractRequest> newContracts, CancellationToken cancellationToken)
        {
            var existingContracts = await dbContext.SupplierContracts
                .Where(sc => sc.SupplierId == supplierId && !sc.IsCancelled)
                .ToListAsync(cancellationToken);

            // Get the IDs of contracts that should remain (from the request)
            var contractIdsToKeep = newContracts.Select(c => c.Id).ToHashSet();

            // Mark contracts that are not in the request as cancelled (deleted)
            foreach (var existingContract in existingContracts)
            {
                if (!contractIdsToKeep.Contains(existingContract.Id))
                {
                    // When a contract is soft-deleted, it still exists in the database with the same primary key (Guid),
                    // so when try to add a contract with the same ID, it will cause a primary key constraint violation.
                    dbContext.SupplierContracts.Remove(existingContract);
                }
            }

            // Update existing contracts and add new ones
            foreach (var contractRequest in newContracts)
            {
                var existingContract = existingContracts.FirstOrDefault(c => c.Id == contractRequest.Id);

                if (existingContract != null)
                {
                    existingContract.FileName = contractRequest.FileName;
                }
                else
                {
                    var newContract = new SupplierContract
                    {
                        Id = contractRequest.Id,
                        FileName = contractRequest.FileName,
                        SupplierId = supplierId,
                        IsCancelled = false,
                        IsSuspended = false,
                    };
                    dbContext.SupplierContracts.Add(newContract);
                }
            }
        }
    }
}