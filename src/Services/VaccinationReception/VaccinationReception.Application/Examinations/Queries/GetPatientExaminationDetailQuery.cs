using BuildingBlocks.CQRS;
using BuildingBlocks.Strings.Enums;
using VaccinationReception.Domain.Models;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetPatientExaminationDetailQuery(int ExaminationId) : IQuery<GetPatientExaminationDetailQueryResponse>;

    public record GetPatientExaminationDetailQueryResponse
    {
        public string? Diagnose { get; init; }
        public DateTime? ReceptionTime { get; init; }
        public int? PerformTechnicianId { get; init; }
        public string? PerformTechnicianName { get; init; }
        public SampleType? SampleType { get; init; }
        public SampleQualityLevel? SampleQuality { get; init; }
        public DateTime? ExecutionTime { get; init; }
        public int? DoctorId { get; init; }
        public string? DoctorName { get; init; }
        public string? Conclusion { get; init; }
        public string? Note { get; init; }
    }
}
