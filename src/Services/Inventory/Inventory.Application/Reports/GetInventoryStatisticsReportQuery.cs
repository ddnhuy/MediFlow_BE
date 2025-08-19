// src/Services/Inventory/Inventory.Application/Reports/GetInventoryStatisticsReportQuery.cs
using HumanResource.Grpc;
using Inventory.Application.Helpers;
using System.Security.Claims;

namespace Inventory.Application.Reports
{
    public record GetInventoryStatisticsReportQuery(
        DateOnly? FromDate = null,
        DateOnly? ToDate = null,
        string? VaccineCategory = null) : IQuery<InventoryStatisticsReportDTO>;

    public class GetInventoryStatisticsReportQueryHandler : IQueryHandler<GetInventoryStatisticsReportQuery, InventoryStatisticsReportDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProtoServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        // Constants from GetInventoryLimitStockQueryHandler
        private const int CRITICAL_LOW = -100;
        private const int NORMAL = 0;

        public GetInventoryStatisticsReportQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ApplicationUserProtoService.ApplicationUserProtoServiceClient applicationUserProtoServiceClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _applicationUserProtoServiceClient = applicationUserProtoServiceClient;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        public async Task<InventoryStatisticsReportDTO> Handle(GetInventoryStatisticsReportQuery request, CancellationToken cancellationToken)
        {
            // Set default date range if not provided
            var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = request.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // Get inventory limit stocks for status calculation
            var inventoryLimitStocks = await _context.InventoryLimitStocks
                .AsNoTracking()
                .Where(ils => !ils.IsSuspended && !ils.IsCancelled)
                .ToDictionaryAsync(ils => ils.MedicineId, ils => ils.MinimalStockThreshold, cancellationToken);

            // Get current inventory details
            var inventoryData = await _context.InventoryDetails
                .AsNoTracking()
                .Where(id => !id.IsSuspended && !id.IsCancelled)
                .Include(id => id.MedicineBatch)
                    .ThenInclude(mb => mb.Medicine)
                        .ThenInclude(m => m.MedicinePrice)
                .Include(id => id.MedicineBatch)
                    .ThenInclude(mb => mb.Supplier)
                .Where(id => id.MedicineBatch!.Status == BuildingBlocks.Strings.Enums.MedicineBatchStatus.IsActive)
                .Select(id => new
                {
                    id.Quantity,
                    id.CostPrice,
                    VaccineCode = id.MedicineBatch!.Medicine!.MedicineCode ?? "",
                    VaccineName = id.MedicineBatch!.Medicine!.MedicineName ?? "",
                    Unit = id.MedicineBatch!.Medicine!.Unit ?? "",
                    Classification = id.MedicineBatch!.Medicine!.MedicineClassification ?? "",
                    UnitPrice = id.MedicineBatch!.Medicine!.MedicinePrice != null
                        ? id.MedicineBatch.Medicine.MedicinePrice.UnitPrice
                        : id.CostPrice,
                    SupplierName = id.MedicineBatch!.Supplier!.SupplierName ?? "",
                    BatchNumber = id.MedicineBatch!.BatchNumber,
                    ExpiryDate = id.MedicineBatch!.ExpiryDate,
                    MedicineId = id.MedicineBatch!.MedicineId,
                    BatchId = id.MedicineBatchId
                })
                .ToListAsync(cancellationToken);

            // Filter by category if specified
            if (!string.IsNullOrEmpty(request.VaccineCategory))
            {
                inventoryData = inventoryData
                    .Where(i => i.Classification.Contains(request.VaccineCategory, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // 1. Vaccine Stock Summary (grouped by vaccine type)
            var vaccineStocks = inventoryData
                .GroupBy(i => new { i.MedicineId, i.VaccineCode, i.VaccineName, i.Unit, i.Classification })
                .Select((g, index) => new VaccineStockDTO
                {
                    Stt = index + 1,
                    VaccineCode = g.Key.VaccineCode,
                    VaccineName = g.Key.VaccineName,
                    Unit = g.Key.Unit,
                    Classification = g.Key.Classification,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    AverageUnitPrice = g.Average(x => x.UnitPrice),
                    TotalValue = g.Sum(x => x.Quantity * x.UnitPrice),
                    BatchCount = g.Count(),
                    NearestExpiry = g.Min(x => x.ExpiryDate),
                    Status = GetStockStatus(
                        g.Sum(x => x.Quantity),
                        g.Min(x => x.ExpiryDate),
                        g.Key.MedicineId,
                        inventoryLimitStocks)
                })
                .OrderByDescending(v => v.TotalValue)
                .ToList();

            // 2. Batch Details (all individual batches)
            var batchDetails = inventoryData
                .Select((i, index) => new Inventory.Application.DTOs.InventoryMedicineBatchDetailDTO
                {
                    Stt = index + 1,
                    VaccineCode = i.VaccineCode,
                    VaccineName = i.VaccineName,
                    BatchNumber = i.BatchNumber,
                    SupplierName = i.SupplierName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalValue = i.Quantity * i.UnitPrice,
                    ExpiryDate = i.ExpiryDate,
                    DaysToExpiry = (i.ExpiryDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days,
                    Status = GetBatchStatus(i.Quantity, i.ExpiryDate)
                })
                .OrderBy(b => b.VaccineCode)
                .OrderByDescending(b => b.DaysToExpiry)
                .ToList();

            // 3. Inventory Transactions (history within date range)
            // 3. Inventory Transactions (history within date range)
            var transactionData = await _context.InventoryHistories
                .AsNoTracking()
                .Where(h => h.TransactionDate >= fromDateTime && h.TransactionDate <= toDateTime
                    && !h.IsSuspended && !h.IsCancelled)
                .Include(h => h.Medicine)
                .Include(h => h.MedicineBatch)
                .Select(h => new
                {
                    h.TransactionDate,
                    TransactionType = h.TransactionType,
                    VaccineCode = h.Medicine!.MedicineCode ?? "",
                    VaccineName = h.Medicine!.MedicineName ?? "",
                    BatchNumber = h.MedicineBatch != null ? h.MedicineBatch.BatchNumber : "",
                    h.Quantity,
                    h.UnitPrice,
                    TotalValue = h.Quantity * h.UnitPrice,
                    Description = h.Description ?? ""
                })
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(cancellationToken);

            // Convert to DTO with index after database query
            var transactions = transactionData
                .Select((t, index) => new InventoryTransactionDTO
                {
                    Stt = index + 1,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType == InventoryTransactionType.IMPORT ? "Nhập kho" : "Xuất kho",
                    VaccineCode = t.VaccineCode,
                    VaccineName = t.VaccineName,
                    BatchNumber = t.BatchNumber,
                    Quantity = t.Quantity,
                    UnitPrice = t.UnitPrice,
                    TotalValue = t.TotalValue,
                    Description = t.Description
                })
                .ToList();

            // Calculate summary statistics
            var summary = new InventorySummaryDTO
            {
                TotalVaccineTypes = vaccineStocks.Count,
                TotalQuantityInStock = vaccineStocks.Sum(v => v.TotalQuantity),
                TotalInventoryValue = vaccineStocks.Sum(v => v.TotalValue),
                TotalBatches = batchDetails.Count,
                BatchesNearExpiry = batchDetails.Count(b => b.DaysToExpiry <= 30 && b.DaysToExpiry > 0),
                LowStockVaccines = vaccineStocks.Count(v => v.Status == "Tồn kho thấp" || v.Status == "Thiếu nghiêm trọng")
            };

            // Get current user info
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var currentUser = await _applicationUserProtoServiceClient.GetApplicationUserAsync(
                    new GetApplicationUserRequest { Id = _currentUserService.UserId },
                    metadata);

            return new InventoryStatisticsReportDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = currentUser.Name ?? "Admin",
                Summary = summary,
                VaccineStocks = vaccineStocks,
                BatchDetails = batchDetails,
                Transactions = transactions
            };
        }

        private static string GetStockStatus(decimal currentStock, DateOnly expiryDate, int medicineId, Dictionary<int, decimal> inventoryLimitStocks)
        {
            // Check expiry first (highest priority)
            var daysToExpiry = (expiryDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days;
            if (daysToExpiry <= 0) return "Hết hạn";
            if (daysToExpiry <= 30) return "Gần hết hạn";

            // Check stock level using limit stock threshold
            if (inventoryLimitStocks.TryGetValue(medicineId, out var threshold))
            {
                var difference = currentStock - threshold;

                return difference switch
                {
                    < CRITICAL_LOW => "Thiếu nghiêm trọng",
                    < NORMAL => "Tồn kho thấp",
                    >= NORMAL => "Bình thường"
                };
            }

            // Fallback to default logic if no threshold defined
            if (currentStock <= 10) return "Tồn kho thấp";
            return "Bình thường";
        }

        private static string GetBatchStatus(decimal quantity, DateOnly expiryDate)
        {
            var daysToExpiry = (expiryDate.ToDateTime(TimeOnly.MinValue) - DateTime.UtcNow).Days;

            if (daysToExpiry <= 0) return "Hết hạn";
            if (daysToExpiry <= 7) return "Sắp hết hạn";
            if (daysToExpiry <= 30) return "Gần hết hạn";
            if (quantity == 0) return "Hết hàng";
            return "Bình thường";
        }
    }
}