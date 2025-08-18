using Inventory.Application.Reports;

namespace Inventory.API.Endpoints
{
    public class GetMedicineRevenueReportsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-revenue", async (ISender mediator, [AsParameters] GetMedicineRevenueReportQuery query) =>
            {
                if ((query.FromDate.HasValue && query.ToDate.HasValue) && (query.FromDate > query.ToDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var result = await mediator.Send(query);
                return Results.Ok(result);
            }).RequireAuthorization()
               .WithName("GetMedicineRevenueReports")
              .WithSummary("Get Medicine Revenue Reports")
              .WithDescription("Retrieves revenue reports for medicines based on specified date range and optional filters.")
              .Produces<MedicineRevenueReportDTO>()
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .ProducesProblem(StatusCodes.Status404NotFound)
              .WithTags("Reports");
        }
    }
}
