using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetTodayRevenueResult(double Amount, string currency);
    public record GetTodayRevenueQuery : IQuery<GetTodayRevenueResult>;

    internal class GetTodayRevenueQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetTodayRevenueQuery, GetTodayRevenueResult>
    {
        public async Task<GetTodayRevenueResult> Handle(GetTodayRevenueQuery request, CancellationToken cancellationToken)
        {
            var response = await vaccinationReceptionProto.GetTodayRevenueAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            return new GetTodayRevenueResult(response.Amount, response.Currency);
        }
    }
}
