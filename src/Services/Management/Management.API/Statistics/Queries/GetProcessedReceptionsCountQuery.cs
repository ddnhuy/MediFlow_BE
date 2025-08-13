using VaccinationReception.API;

namespace Management.API.Statistics.Queries
{
    public record GetProcessedReceptionsCountQueryResult(long Count);
    public record GetProcessedReceptionsCountQuery() : IQuery<GetProcessedReceptionsCountQueryResult>;

    internal class GetProcessedReceptionsCountQueryHandler(
        VaccinationReceptionProtoService.VaccinationReceptionProtoServiceClient vaccinationReceptionProto) : IQueryHandler<GetProcessedReceptionsCountQuery, GetProcessedReceptionsCountQueryResult>
    {
        public async Task<GetProcessedReceptionsCountQueryResult> Handle(GetProcessedReceptionsCountQuery request, CancellationToken cancellationToken)
        {
            var response = await vaccinationReceptionProto.GetProcessedReceptionsCountAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            return new GetProcessedReceptionsCountQueryResult(response.Count);
        }
    }
}