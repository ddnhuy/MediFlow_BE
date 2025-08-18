using Inventory.Application.Reports;

namespace Inventory.API.Endpoints
{
    public class GetInventoryStatisticsReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory-statistics", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                string? vaccineCategory,
                ISender sender) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetInventoryStatisticsReportQuery(fromDate, toDate, vaccineCategory);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetInventoryStatisticsReport")
            .WithSummary("Get inventory statistics report")
            .WithDescription("Retrieve comprehensive inventory statistics including vaccine stocks, batch details, and transactions")
            .Produces<InventoryStatisticsReportDTO>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}