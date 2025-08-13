namespace Management.API.Dtos.Statistic
{
    public class MonthlyRevenueDto
    {
        public string Month { get; set; } = default!;
        public double TotalRevenue { get; set; } = 0.0;
        public string Currency { get; set; } = default!;
    }
}
