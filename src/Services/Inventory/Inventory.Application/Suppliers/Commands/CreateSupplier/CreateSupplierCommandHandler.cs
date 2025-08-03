using System.Security.Cryptography;

namespace Inventory.Application.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateSupplierCommand, CreateSupplierResult>
    {
        public async Task<CreateSupplierResult> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
        {
            var supplierCode = await GenerateUniqueSupplierCodeAsync(cancellationToken);

            var supplier = new Supplier
            {
                SupplierCode = supplierCode,
                SupplierName = request.SupplierName,
                Address = request.Address,
                Phone = request.Phone,
                Fax = request.Fax,
                Email = request.Email,
                TaxCode = request.TaxCode,
                Director = request.Director,
                ContactPerson = request.ContactPerson,
                ExpiredDate = request.ExpiredDate,
                IsCancelled = false,
                IsSuspended = false,
            };

            // Check for contracts that already belong to another supplier
            if (request.Contracts != null && request.Contracts.Any())
            {
                var contractIds = request.Contracts.Select(c => c.Id).ToList();

                // Explicit Guid comparison
                var conflictingContracts = await dbContext.SupplierContracts
                    .Where(sc => contractIds.Contains(sc.Id) && !sc.IsCancelled)
                    .ToListAsync(cancellationToken);

                if (conflictingContracts.Any())
                {
                    throw new BadRequestException(
                        ExceptionKey.CONTRACT_IS_ALREADY_BELONG_TO_ANOTHER_SUPPLIER
                    );
                }
            }

            var result = dbContext.Suppliers.Add(supplier);
            await dbContext.SaveChangesAsync(cancellationToken);

            if (request.Contracts != null && request.Contracts.Any())
            {
                var supplierContracts = request.Contracts.Select(contract => new SupplierContract
                {
                    Id = contract.Id,
                    FileName = contract.FileName,
                    SupplierId = supplier.Id,
                    IsCancelled = false,
                    IsSuspended = false,
                }).ToList();

                dbContext.SupplierContracts.AddRange(supplierContracts);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return new CreateSupplierResult(supplier.Id);
        }

        /// <summary>
        /// Generate a unique supplier code with format: SUP_XXXXXXXX
        /// Using timestamp and random bytes to ensure uniqueness
        /// Combine timestamp and random bytes to create a unique 8-character string
        /// Ensure it's exactly 8 characters by taking the last 8 characters
        /// </summary>
        /// <returns>Unique supplier code</returns>
        private async Task<string> GenerateUniqueSupplierCodeAsync(CancellationToken cancellationToken)
        {
            string supplierCode;
            bool isDuplicate;
            int maxAttempts = 10; // Prevent infinite loop
            int attempts = 0;

            do
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var randomBytes = new byte[4];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                var combinedValue = timestamp ^ BitConverter.ToUInt32(randomBytes, 0);
                var uniquePart = Convert.ToString(combinedValue, 16).PadLeft(8, '0').ToUpper();

                if (uniquePart.Length > 8)
                {
                    uniquePart = uniquePart.Substring(uniquePart.Length - 8);
                }

                supplierCode = $"SUP_{uniquePart}";

                // Check if the generated code already exists
                isDuplicate = await dbContext.Suppliers
                    .AnyAsync(x => x.SupplierCode == supplierCode && !x.IsCancelled, cancellationToken);

                attempts++;
            } while (isDuplicate && attempts < maxAttempts);

            return supplierCode;
        }
    }
}
