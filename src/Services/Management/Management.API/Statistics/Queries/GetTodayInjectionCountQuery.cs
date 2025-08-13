using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetTodayInjectionCountQueryResult(long Count);
    public record GetTodayInjectionCountQuery() : IQuery<GetTodayInjectionCountQueryResult>;

    internal class GetTodayInjectionCountQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetTodayInjectionCountQuery, GetTodayInjectionCountQueryResult>
    {
        public async Task<GetTodayInjectionCountQueryResult> Handle(GetTodayInjectionCountQuery request, CancellationToken cancellationToken)
        {
            var response = await vaccinationReceptionProto.GetTodayInjectionCountAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            return new GetTodayInjectionCountQueryResult(response.Count);
        }
    }
}