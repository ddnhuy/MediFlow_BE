namespace Inventory.Application.DTOs;

public class MedicineRevenueReportDTO
{
    public DateOnly FromDate { get; set; }
    public DateOnly ToDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public string GeneratedBy { get; set; } = string.Empty;

    public ReportSummaryDTO Summary { get; set; } = new();
    public List<MedicineRevenueDetailDTO> MedicineDetails { get; set; } = new();
    public List<CategoryStatisticDTO> CategoryStatistics { get; set; } = new();
    public List<DailyStatisticDTO> DailyStatistics { get; set; } = new();
    public List<Inventory.Application.DTOs.BatchDetailDTO> BatchDetails { get; set; } = new();
}

public class ReportSummaryDTO
{
    public decimal TotalRevenue { get; set; }
    public int TotalQuantityUsed { get; set; }
    public int TotalMedicineTypes { get; set; }
    public int TotalBatchesUsed { get; set; }
    public decimal AverageUnitPrice { get; set; }
    public decimal EstimatedProfit { get; set; }
}

public class MedicineRevenueDetailDTO
{
    public int Stt { get; set; }
    public string MedicineCode { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Classification { get; set; } = string.Empty;
    public int QuantityUsed { get; set; }
    public decimal AverageUnitPrice { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageCostPrice { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal EstimatedProfit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class CategoryStatisticDTO
{
    public string Category { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Revenue { get; set; }
    public decimal Percentage { get; set; }
    public decimal EstimatedProfit { get; set; }
    public decimal ProfitMargin { get; set; }
}

public class DailyStatisticDTO
{
    public DateTime Date { get; set; }
    public int QuantityUsed { get; set; }
    public decimal Revenue { get; set; }
    public int MedicineTypeCount { get; set; }
}

public class BatchDetailDTO
{
    public string MedicineName { get; set; } = string.Empty;
    public string MedicineCode { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }
    public int QuantityUsed { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Revenue { get; set; }
    public decimal Profit { get; set; }
}