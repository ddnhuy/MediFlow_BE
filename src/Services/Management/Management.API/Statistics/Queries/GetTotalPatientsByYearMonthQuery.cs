using Management.API.Dtos.Statistic;
using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetTotalPatientsByYearMonthResult(IEnumerable<YearlyPatientDto> YearlyPatientList);
    public record GetTotalPatientsByYearMonthQuery : IQuery<GetTotalPatientsByYearMonthResult>;

    internal class GetTotalPatientsByYearMonthQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetTotalPatientsByYearMonthQuery, GetTotalPatientsByYearMonthResult>
    {
        public async Task<GetTotalPatientsByYearMonthResult> Handle(GetTotalPatientsByYearMonthQuery request, CancellationToken cancellationToken)
        {
            var yearlyPatientList = await vaccinationReceptionProto.GetTotalPatientsByYearMonthAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            var result = yearlyPatientList.Data.Select(yearlyPatient => new YearlyPatientDto
            {
                Year = yearlyPatient.Year,
                MonthlyPatients = yearlyPatient.Months.Select(monthlyPatient => new MonthlyPatientDto
                {
                    Month = monthlyPatient.Month,
                    TotalPatients = monthlyPatient.TotalPatients
                }).ToList()
            }).ToList();

            return new GetTotalPatientsByYearMonthResult(result);
        }
    }
}