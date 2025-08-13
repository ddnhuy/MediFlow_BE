namespace Management.API.Dtos.Statistic
{
    public class VaccineTrafficDto
    {
        public int VaccineId { get; set; }
        public string VaccineName { get; set; } = default!;
        public long TotalUsed { get; set; }
    }
}
