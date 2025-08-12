using BuildingBlocks.Strings.Enums;

namespace Inventory.Application.Medicines.Queries.GetMedicineBatchesByMedicineId
{
    public class GetMedicineBatchesByMedicineIdQueryHandler : IQueryHandler<GetMedicineBatchesByMedicineIdQuery, GetMedicineBatchesByMedicineIdResult>
    {
        private readonly IApplicationDbContext _context;

        public GetMedicineBatchesByMedicineIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetMedicineBatchesByMedicineIdResult> Handle(GetMedicineBatchesByMedicineIdQuery request, CancellationToken cancellationToken)
        {
            var medicineExists = await _context.Medicines.AnyAsync(m => m.Id == request.medicineId, cancellationToken);
            if (!medicineExists)
            {
                throw new BadRequestException(ExceptionKey.NOT_FOUND_MEDICINE_WITH_ID);
            }

            var pageIndex = request.PaginationRequest.PageIndex;
            var pageSize = request.PaginationRequest.PageSize;

            var baseQuery = _context.MedicineBatches
                .Where(mb => mb.MedicineId == request.medicineId && mb.Status == MedicineBatchStatus.IsActive);

            if (!string.IsNullOrWhiteSpace(request.batchNumber))
            {
                var search = request.batchNumber.Trim().ToLower();
                baseQuery = baseQuery.Where(mb => mb.BatchNumber.ToLower().Contains(search));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var medicineBatches = await baseQuery
                .Include(mb => mb.Medicine)
                .Include(mb => mb.Supplier)
                .Include(mb => mb.Manufacturer)
                .Select(mb => new MedicineBatchDTO
                {
                    Id = mb.Id,
                    MedicineId = mb.MedicineId,
                    MedicineName = mb.Medicine!.MedicineName,
                    BatchNumber = mb.BatchNumber,
                    ImportDate = mb.ImportDate,
                    ExpiryDate = mb.ExpiryDate,
                    ImportPrice = mb.ImportPrice,
                    SupplierId = mb.SupplierId,
                    SupplierName = mb.Supplier!.SupplierName,
                    ManufacturerId = mb.ManufacturerId,
                    ManufacturerName = mb.Manufacturer!.ManufacturerName,
                    Status = mb.Status
                })
                .OrderByDescending(x => x.ExpiryDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new GetMedicineBatchesByMedicineIdResult(
                new PaginatedResult<MedicineBatchDTO>(pageIndex, pageSize, totalCount, medicineBatches));
        }
    }
}
