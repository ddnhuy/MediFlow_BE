using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetAllExaminationHistoryOfPatientQuery(int PatientID) : IQuery<GetAllExaminationHistoryOfPatientResponse>;

    public record GetAllExaminationHistoryOfPatientResponse(
        int PatientId,
        string? PatientCode,
        string? PatientName,
        DateTime DOB,
        string? PhoneNumber,
        DateTime ReturnDate,
        List<ExaminationHistoryOfPatientItem> ExaminationHistory
    );

    public class ExaminationHistoryOfPatientItem
    {
        public int ExaminationId { get; set; }
        public DateTime ReturnTime { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
