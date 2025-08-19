using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Reports;

namespace VaccinationReception.API.EndPoints.Reports
{
    public class GetHospitalRevenueReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/hospital-revenue", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                ISender sender) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetHospitalRevenueReportQuery(fromDate, toDate);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetHospitalRevenueReport")
            .WithSummary("Get hospital revenue report")
            .WithDescription("Retrieve hospital revenue statistics including exam fees, test fees, and vaccination fees")
            .Produces<HospitalRevenueReportDTO>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}