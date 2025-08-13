using Management.API.Dtos.Statistic;
using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetTotalRevenueByYearMonthResult(IEnumerable<YearlyRevenueDto> YearlyRevenueList);
    public record GetTotalRevenueByYearMonthQuery : IQuery<GetTotalRevenueByYearMonthResult>;

    internal class GetTotalRevenueByYearMonthQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetTotalRevenueByYearMonthQuery, GetTotalRevenueByYearMonthResult>
    {
        public async Task<GetTotalRevenueByYearMonthResult> Handle(GetTotalRevenueByYearMonthQuery request, CancellationToken cancellationToken)
        {
            var yearlyRevenueList = await vaccinationReceptionProto.GetTotalRevenueByYearMonthAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            var result = yearlyRevenueList.Data.Select(yearlyRevenue => new YearlyRevenueDto
            {
                Year = yearlyRevenue.Year,
                MonthlyRevenues = yearlyRevenue.Months.Select(monthlyRevenue => new MonthlyRevenueDto
                {
                    Month = monthlyRevenue.Month,
                    TotalRevenue = monthlyRevenue.TotalRevenue,
                    Currency = monthlyRevenue.Currency
                }).ToList()
            }).ToList();

            return new GetTotalRevenueByYearMonthResult(result);
        }
    }
}
