using BuildingBlocks.CQRS;

namespace VaccinationReception.Application.Vaccinations.Queries.GetPendingVaccinationsTodayQuery
{
    public record GetPendingVaccinationsTodayQuery(
        int ReceptionId
    ) : IQuery<GetPendingVaccinationsTodayResult>;

    public record GetPendingVaccinationsTodayResult(
        int TotalPendingDoses,
        List<PendingVaccinationDto> PendingVaccinations
    );

    public record PendingVaccinationDto(
        int ReceptionVaccinationId,
        int VaccineId,
        string VaccineName,
        int TotalQuantity,
        int CompletedDoses,
        int PendingDoses,
        DateTime? ScheduledDate
    );
}
