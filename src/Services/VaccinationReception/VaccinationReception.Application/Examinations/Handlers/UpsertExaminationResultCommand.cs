using BuildingBlocks.CQRS;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Examinations.Handlers
{
    public record UpsertExaminationResultCommand(List<ExaminationTestResultUpsertDTO> Results) : ICommand<UpsertExaminationResult>;

    public record ExaminationTestResultUpsertDTO()
    {
        public int ExaminationId { get; set; }
        public int PatientId { get; set; }
        public string Diagnose { get; set; } = string.Empty;
        public DateTime ReturnTime { get; set; }
        public int PerformTechnicianId { get; set; }    
        public SampleType SampleType { get; set; }
        public SampleQualityLevel SampleQuality { get; set; }
        public int DoctorId { get; set; }
        public string Conclusion { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public List<ExaminationResultItem> ExaminationResults { get; set; } = new List<ExaminationResultItem>();
    }

    public record ExaminationResultItem()
    {
        public string? ParameterName { get; set; } = string.Empty;
        public string ResultValue { get; set; } = string.Empty;
        public string StandardValue { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }

    public record UpsertExaminationResult(bool IsSuccess);
}
