namespace Management.API.Dtos.Statistic
{
    public class MonthlyPatientDto
    {
        public string Month { get; set; } = default!;
        public long TotalPatients { get; set; } = 0;
    }
}
