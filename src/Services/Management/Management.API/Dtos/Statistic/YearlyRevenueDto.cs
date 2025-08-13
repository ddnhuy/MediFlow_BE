namespace Management.API.Dtos.Statistic
{
    public class YearlyRevenueDto
    {
        public int Year { get; set; }
        public IEnumerable<MonthlyRevenueDto> MonthlyRevenues { get; set; } = [];
    }
}
