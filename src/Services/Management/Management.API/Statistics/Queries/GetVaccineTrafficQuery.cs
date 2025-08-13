using Inventory.API;
using Management.API.Dtos.Statistic;

namespace Management.API.Statistics.Queries
{
    public record GetVaccineTrafficQueryResult(IEnumerable<VaccineTrafficDto> VaccineTraffic);
    public record GetVaccineTrafficQuery() : IQuery<GetVaccineTrafficQueryResult>;

    internal class GetVaccineTrafficQueryHandler(
        InventoryProtoService.InventoryProtoServiceClient inventoryProto) : IQueryHandler<GetVaccineTrafficQuery, GetVaccineTrafficQueryResult>
    {
        public async Task<GetVaccineTrafficQueryResult> Handle(GetVaccineTrafficQuery request, CancellationToken cancellationToken)
        {
            var vaccineTraffics = await inventoryProto.GetTrafficByVaccineAsync(new GetTrafficByVaccineRequest(), cancellationToken: cancellationToken);

            return new GetVaccineTrafficQueryResult(vaccineTraffics.Data.Select(t => new VaccineTrafficDto
            {
                VaccineId = t.VaccineId,
                VaccineName = t.VaccineName,
                TotalUsed = t.TotalUsed
            }).ToList());
        }
    }
}
