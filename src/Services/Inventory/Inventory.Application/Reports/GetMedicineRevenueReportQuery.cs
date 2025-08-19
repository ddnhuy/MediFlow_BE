using HumanResource.Grpc;
using Inventory.Application.Helpers;

namespace Inventory.Application.Reports
{
    public record GetMedicineRevenueReportQuery(
        DateOnly? FromDate = null,
        DateOnly? ToDate = null) : IQuery<MedicineRevenueReportDTO>;

    public class GetMedicineRevenueReportQueryHandler : IQueryHandler<GetMedicineRevenueReportQuery, MedicineRevenueReportDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly ApplicationUserProtoService.ApplicationUserProtoServiceClient _applicationUserProtoServiceClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public GetMedicineRevenueReportQueryHandler(
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

        public async Task<MedicineRevenueReportDTO> Handle(GetMedicineRevenueReportQuery request, CancellationToken cancellationToken)
        {
            // Set default date range if not provided
            var fromDate = request.FromDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
            var toDate = request.ToDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // First, get the raw data without complex grouping
            var inventoryHistoryData = await _context.InventoryHistories
                .AsNoTracking()
                .Where(h => h.TransactionType == InventoryTransactionType.EXPORT
                    && h.TransactionDate >= fromDateTime
                    && h.TransactionDate <= toDateTime
                    && !h.IsSuspended && !h.IsCancelled)
                .Include(h => h.Medicine)
                    .ThenInclude(m => m.MedicinePrice) // Include MedicinePrice
                .Include(h => h.MedicineBatch)
                    .ThenInclude(mb => mb.Supplier)
                .Select(h => new
                {
                    h.MedicineId,
                    h.MedicineBatchId,
                    h.Quantity,
                    h.UnitPrice, // This might be the actual selling price at transaction time
                    h.TransactionDate,
                    MedicineCode = h.Medicine!.MedicineCode ?? "",
                    MedicineName = h.Medicine!.MedicineName ?? "",
                    Unit = h.Medicine!.Unit ?? "",
                    Classification = h.Medicine!.MedicineClassification ?? "",
                    // Use ImportPrice as Cost Price (since cost price is similar to import price)
                    CostPrice = h.MedicineBatch!.ImportPrice,
                    // Get selling price from MedicinePrice table
                    SellingPrice = h.Medicine!.MedicinePrice != null ? h.Medicine.MedicinePrice.UnitPrice : h.UnitPrice,
                    SupplierName = h.MedicineBatch!.Supplier!.SupplierName ?? "",
                    BatchNumber = h.MedicineBatch!.BatchNumber,
                    ExpiryDate = h.MedicineBatch!.ExpiryDate,
                    ImportPrice = h.MedicineBatch!.ImportPrice,
                })
                .ToListAsync(cancellationToken);

            // Group in memory to create medicine details
            var medicineDetails = inventoryHistoryData
                .GroupBy(h => new { h.MedicineId, h.MedicineCode, h.MedicineName, h.Unit, h.Classification })
                .Select((g, index) => new MedicineRevenueDetailDTO
                {
                    Stt = index + 1,
                    MedicineCode = g.Key.MedicineCode,
                    MedicineName = g.Key.MedicineName,
                    Unit = g.Key.Unit,
                    Classification = g.Key.Classification,
                    QuantityUsed = (int)g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.SellingPrice), // Use selling price from MedicinePrice
                    AverageUnitPrice = g.Average(x => x.SellingPrice), // Average selling price
                    AverageCostPrice = g.Average(x => x.CostPrice), // Average import price (cost price)
                    SupplierName = g.First().SupplierName,
                    EstimatedProfit = g.Sum(x => x.Quantity * (x.SellingPrice - x.CostPrice)), // Profit = (Selling - Cost) * Quantity
                    ProfitMargin = g.Sum(x => x.Quantity * x.SellingPrice) > 0
                        ? (g.Sum(x => x.Quantity * (x.SellingPrice - x.CostPrice)) / g.Sum(x => x.Quantity * x.SellingPrice)) * 100
                        : 0
                })
                .OrderByDescending(m => m.TotalRevenue)
                .ToList();

            // Summary calculations
            var summary = new ReportSummaryDTO
            {
                TotalRevenue = medicineDetails.Sum(m => m.TotalRevenue),
                TotalQuantityUsed = medicineDetails.Sum(m => m.QuantityUsed),
                TotalMedicineTypes = medicineDetails.Count,
                AverageUnitPrice = medicineDetails.Any() ? medicineDetails.Average(m => m.AverageUnitPrice) : 0,
                EstimatedProfit = medicineDetails.Sum(m => m.EstimatedProfit),
                TotalBatchesUsed = inventoryHistoryData.Select(h => h.MedicineBatchId).Distinct().Count()
            };

            // Category statistics
            var categoryStats = medicineDetails
                .GroupBy(m => m.Classification)
                .Select(g => new CategoryStatisticDTO
                {
                    Category = g.Key,
                    Quantity = g.Sum(x => x.QuantityUsed),
                    Revenue = g.Sum(x => x.TotalRevenue),
                    EstimatedProfit = g.Sum(x => x.EstimatedProfit),
                    Percentage = summary.TotalRevenue > 0 ? (g.Sum(x => x.TotalRevenue) / summary.TotalRevenue) * 100 : 0,
                    ProfitMargin = g.Sum(x => x.TotalRevenue) > 0 ? (g.Sum(x => x.EstimatedProfit) / g.Sum(x => x.TotalRevenue)) * 100 : 0
                })
                .OrderByDescending(c => c.Revenue)
                .ToList();

            // Daily statistics - using selling price
            var dailyStats = await _context.InventoryHistories
                .AsNoTracking()
                .Where(h => h.TransactionType == InventoryTransactionType.EXPORT
                    && h.TransactionDate >= fromDateTime
                    && h.TransactionDate <= toDateTime
                    && !h.IsSuspended && !h.IsCancelled)
                .Include(h => h.Medicine)
                    .ThenInclude(m => m.MedicinePrice)
                .GroupBy(h => h.TransactionDate.Date)
                .Select(g => new DailyStatisticDTO
                {
                    Date = g.Key,
                    QuantityUsed = (int)g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * (x.Medicine!.MedicinePrice != null ? x.Medicine.MedicinePrice.UnitPrice : x.UnitPrice)),
                    MedicineTypeCount = g.Select(x => x.MedicineId).Distinct().Count()
                })
                .OrderBy(d => d.Date)
                .ToListAsync(cancellationToken);

            // Batch details - group in memory with correct prices
            var batchDetails = inventoryHistoryData
                .GroupBy(h => new { h.MedicineId, h.MedicineBatchId, h.MedicineName, h.MedicineCode, h.BatchNumber, h.ExpiryDate, h.ImportPrice })
                .Select(g => new BatchDetailDTO
                {
                    MedicineName = g.Key.MedicineName,
                    MedicineCode = g.Key.MedicineCode,
                    BatchNumber = g.Key.BatchNumber,
                    ExpiryDate = g.Key.ExpiryDate,
                    QuantityUsed = (int)g.Sum(x => x.Quantity),
                    ImportPrice = g.Key.ImportPrice, // This is the cost price
                    SellingPrice = g.Average(x => x.SellingPrice), // Average selling price from MedicinePrice
                    Revenue = g.Sum(x => x.Quantity * x.SellingPrice),
                    Profit = g.Sum(x => x.Quantity * (x.SellingPrice - x.ImportPrice)) // Profit = (Selling - Import) * Quantity
                })
                .OrderBy(b => b.MedicineName)
                .ThenBy(b => b.BatchNumber)
                .ToList();

            // Get current user info
            var rolesClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var metadata = GrpcMetaDataHelper.CreateAuthMetadata(roles: rolesClaim);
            var currentUser = await _applicationUserProtoServiceClient.GetApplicationUserAsync(
                    new GetApplicationUserRequest { Id = _currentUserService.UserId },
                    metadata);

            return new MedicineRevenueReportDTO
            {
                FromDate = fromDate,
                ToDate = toDate,
                GeneratedAt = DateTime.UtcNow,
                GeneratedBy = currentUser.Name ?? "Admin",
                Summary = summary,
                MedicineDetails = medicineDetails,
                CategoryStatistics = categoryStats,
                DailyStatistics = dailyStats,
                BatchDetails = batchDetails
            };
        }
    }
}
