using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationPatient
{
    public record GetListPostVaccinationPatientQuery(string? PatientVaccinationCode, string? PatientName = null) 
        : IQuery<List<GetListPostVaccinationPatientQueryResult>>;

    public record GetListPostVaccinationPatientQueryResult
    (
        int ReceptionId,
        string PatientVaccinationCode,
        string PatientName,
        DateOnly YearOfBirth,
        string PatientCode,
        string Gender
    );
}
