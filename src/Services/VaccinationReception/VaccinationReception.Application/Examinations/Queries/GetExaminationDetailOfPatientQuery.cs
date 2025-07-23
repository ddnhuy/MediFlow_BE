using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetExaminationDetailOfPatientQuery(int ExaminationId) : IQuery<GetExaminationDetailOfPatientQueryResponse>;

    public record GetExaminationDetailOfPatientQueryResponse
    (
      int PatientId,
      string PatientCode,
      string PatientName,
      string PatientPhoneNumber,
      DateTime ReturnDate,
      string ServiceName,
      string Status,
      List<ExaminationTestParameterOfPatient> ExaminationTestParameters,
      string Diagnosis,
      string Conclusion,
      string? Note = ""
    );

    public class ExaminationTestParameterOfPatient
    {
        public string? ParameterName { get; set; } = string.Empty;
        public string? Result { get; set; }  = string.Empty;
        public string? StandardValue { get; set; } = string.Empty;
    }
}
