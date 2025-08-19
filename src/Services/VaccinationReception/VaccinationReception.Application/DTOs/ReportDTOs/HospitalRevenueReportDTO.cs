namespace VaccinationReception.Application.DTOs.Reports
{
    public class HospitalRevenueReportDTO
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;

        public HospitalRevenueSummaryDTO Summary { get; set; } = new();
        public List<DailyRevenueDTO> DailyRevenues { get; set; } = new();
    }

    public class HospitalRevenueSummaryDTO
    {
        public decimal TotalExamFeeRevenue { get; set; }
        public decimal TotalTestFeeRevenue { get; set; }
        public decimal TotalInjectionRevenue { get; set; } 
        public decimal TotalRevenue { get; set; }
        public int TotalExamCount { get; set; }
        public int TotalTestCount { get; set; }
        public int TotalInjectionCount { get; set; } 
        public decimal AverageDailyRevenue { get; set; }
    }

    public class DailyRevenueDTO
    {
        public DateOnly Date { get; set; }
        public decimal ExamFeeRevenue { get; set; }
        public decimal TestFeeRevenue { get; set; }
        public decimal InjectionRevenue { get; set; } 
        public decimal TotalRevenue { get; set; }
        public int ExamCount { get; set; }
        public int TestCount { get; set; }
        public int InjectionCount { get; set; }
    }
}