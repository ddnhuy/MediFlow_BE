namespace VaccinationReception.Application.DTOs.Reports
{
    public class PatientStatisticsReportDTO
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string GeneratedBy { get; set; } = string.Empty;

        public PatientStatisticSummaryDTO Summary { get; set; } = new();
        public List<AgeGroupStatisticDTO> AgeGroupStatistics { get; set; } = new();
        public List<LocationStatisticDTO> LocationStatistics { get; set; } = new();
    }

    public class PatientStatisticSummaryDTO
    {
        public int TotalPatients { get; set; }
    }

    public class AgeGroupStatisticDTO
    {
        public string AgeGroup { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public int PatientCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class LocationStatisticDTO
    {
        public int Stt { get; set; }
        public string Province { get; set; } = string.Empty;
        public int PatientCount { get; set; }
        public decimal Percentage { get; set; }
    }
}