using System.ComponentModel.DataAnnotations.Schema;

namespace VaccinationReception.Domain.Models
{
    public class ExaminationTestResult
    {
        public int ExaminationId { get; set; }
        public Examination Examination { get; set; } = null!;

        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ResultValue { get; set; } = string.Empty;
        public string StandardValue { get; set; } = string.Empty;
    }
}
