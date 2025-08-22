using BuildingBlocks.CQRS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VaccinationReception.Application.Vaccinations.Queries.GetPatientVaccination
{
    public record GetPatientVaccinationQuery(string? SearchTerm = null) : IQuery<GetPatientVaccinationQueryResult>;


    public record GetPatientVaccinationQueryResult(List<PatientVaccinationItem> PatientVaccinationItems);

    public record PatientVaccinationItem(
        int ReceptionId,
        int PatientId,
        string PatientCode,
        string PatientVaccinationCode,
        string PatientName,
        DateTime DateOfBirth,
        string Gender,
        double WeightKg
    );
}
