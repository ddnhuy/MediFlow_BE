using System.ComponentModel.DataAnnotations.Schema;
using VaccinationReception.Domain.Abstractions;

namespace VaccinationReception.Domain.Models
{
    public class ExaminationTestResult : BaseEntity
    {
        public int? ExaminationId { get; set; }
        public Examination? Examination { get; set; } = null!;
        public string? ParameterName { get; set; } = string.Empty;
        public string? ResultValue { get; set; } = string.Empty;
        public string? Unit { get; set; } = string.Empty;
        public string? StandardValue { get; set; } = string.Empty;
    }
}
