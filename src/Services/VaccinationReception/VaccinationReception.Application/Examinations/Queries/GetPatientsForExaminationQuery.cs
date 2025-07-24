using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Examinations.Queries
{
    public record GetPatientsForExaminationQuery(string? PatientName = null) : IQuery<GetPatientsForExaminationResponse>;

    public record GetPatientsForExaminationResponse(List<PatientExaminationInfo> PatientExaminationInfos);

    public record PatientExaminationInfo(
        int ReceptionId,
        int PatientId,
        string PatientName,
        int YearOfBirth,
        string PatientCode,
        int Age,
        string Gender
    );
}
    
