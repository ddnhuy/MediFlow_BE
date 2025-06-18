namespace Inventory.Application.Medicines.Commands.ImportMedicineFromSupplier
{
    public class ImportMedicineFromSupplierCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<ImportMedicineFromSupplierCommand, ImportMedicineFromSupplierResult>
    {
        public async Task<ImportMedicineFromSupplierResult> Handle(ImportMedicineFromSupplierCommand request, CancellationToken cancellationToken)
        {
            // Check duplicate document code and document number
            var documentCodeExists = await dbContext.SupplierImportDocuments
                .AnyAsync(doc => doc.DocumentCode == request.DocumentCode, cancellationToken);

            var documentNumberExists = await dbContext.SupplierImportDocuments
                .AnyAsync(doc => doc.DocumentNumber == request.DocumentNumber, cancellationToken);

            if (documentCodeExists || documentNumberExists)
            {
                throw new BadRequestException(InventoryExceptionStrings.DUPLICATE_DOCUMENT);
            }

            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Create the SupplierImportDocument
                var supplierImportDocument = new SupplierImportDocument
                {
                    DocumentCode = request.DocumentCode,
                    DocumentNumber = request.DocumentNumber,
                    WarehouseId = request.WarehouseId,
                    ImportDate = request.ImportDate,
                    SupplierId = request.SupplierId,
                    Note = request.Note,
                    ReceivedById = request.ReceivedById,
                    SupportingDocument = request.SupportingDocument,
                    EndDate = request.EndDate
                };

                await dbContext.SupplierImportDocuments.AddAsync(supplierImportDocument);
                await dbContext.SaveChangesAsync(cancellationToken);

                // 2. Create the SupplierImportDocumentDetail per Medicine
                foreach (var detail in request.Details)
                {
                    // 2. Process each detail item
                    var medicineBatch = new MedicineBatch
                    {
                        MedicineId = detail.MedicineId,
                        BatchNumber = detail.BatchNumber,
                        ImportDate = request.ImportDate,
                        ExpiryDate = detail.ExpiryDate,
                        ImportPrice = detail.UnitPrice,
                        CostPrice = detail.UnitPrice, // Temporary
                        SupplierId = request.SupplierId,
                        ManufacturerId = detail.ManufacturerId,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request.ReceivedById,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = request.ReceivedById
                    };

                    await dbContext.MedicineBatches.AddAsync(medicineBatch);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    // Create supplier import document detail
                    var supplierImportDocumentDetail = new SupplierImportDocumentDetail
                    {
                        SupplierImportDocumentId = supplierImportDocument.Id,
                        MedicineId = detail.MedicineId,
                        MedicineBatchId = medicineBatch.Id,
                        SGK_CPNK = detail.SGK_CPNK,
                        Note = detail.Note,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        TotalAmount = detail.Quantity * detail.UnitPrice,
                        ExpiryDate = detail.ExpiryDate,
                        ManufacturerId = detail.ManufacturerId,
                        CountryId = detail.CountryId,
                        IsFree = detail.IsFree,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = request.ReceivedById,
                        LastUpdatedAt = DateTime.UtcNow,
                        LastUpdatedBy = request.ReceivedById
                    };

                    await dbContext.SupplierImportDocumentDetails.AddAsync(supplierImportDocumentDetail);

                    medicineBatch.Raise(new InventoryUpdatedEvent(
                        detail.MedicineId,
                        medicineBatch.Id,
                        medicineBatch.BatchNumber,
                        request.WarehouseId,
                        detail.Quantity,
                        detail.UnitPrice,
                        detail.UnitPrice // Temporary cost price = unit price
                    ));

                    await dbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                return new ImportMedicineFromSupplierResult(supplierImportDocument.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }

        }
    }
}