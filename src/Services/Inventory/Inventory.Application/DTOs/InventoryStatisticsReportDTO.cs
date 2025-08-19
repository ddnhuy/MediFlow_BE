namespace Inventory.Application.DTOs
{
    public class InventoryStatisticsReportDTO
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;

        public InventorySummaryDTO Summary { get; set; } = new();
        public List<VaccineStockDTO> VaccineStocks { get; set; } = new();
        public List<InventoryMedicineBatchDetailDTO> BatchDetails { get; set; } = new();
        public List<InventoryTransactionDTO> Transactions { get; set; } = new();
    }

    public class InventorySummaryDTO
    {
        public int TotalVaccineTypes { get; set; }
        public decimal TotalQuantityInStock { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public int TotalBatches { get; set; }
        public int BatchesNearExpiry { get; set; }
        public int LowStockVaccines { get; set; }
    }

    public class VaccineStockDTO
    {
        public int Stt { get; set; }
        public string VaccineCode { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Classification { get; set; } = string.Empty;
        public decimal TotalQuantity { get; set; }
        public decimal AverageUnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public int BatchCount { get; set; }
        public DateOnly? NearestExpiry { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class InventoryMedicineBatchDetailDTO
    {
        public int Stt { get; set; }
        public string VaccineCode { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int DaysToExpiry { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class InventoryTransactionDTO
    {
        public int Stt { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string VaccineCode { get; set; } = string.Empty;
        public string VaccineName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalValue { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}