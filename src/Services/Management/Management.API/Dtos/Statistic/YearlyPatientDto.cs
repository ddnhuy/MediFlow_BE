namespace Management.API.Dtos.Statistic
{
    public class YearlyPatientDto
    {
        public int Year { get; set; } = 2025;
        public IEnumerable<MonthlyPatientDto> MonthlyPatients { get; set; } = [];
    }
}
