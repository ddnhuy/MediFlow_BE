using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetTodayPatientCountQueryResult(long Count);
    public record GetTodayPatientCountQuery() : IQuery<GetTodayPatientCountQueryResult>;

    internal class GetTodayPatientCountQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetTodayPatientCountQuery, GetTodayPatientCountQueryResult>
    {
        public async Task<GetTodayPatientCountQueryResult> Handle(GetTodayPatientCountQuery request, CancellationToken cancellationToken)
        {
            var response = await vaccinationReceptionProto.GetTodayPatientCountAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            return new GetTodayPatientCountQueryResult(response.Count);
        }
    }
}