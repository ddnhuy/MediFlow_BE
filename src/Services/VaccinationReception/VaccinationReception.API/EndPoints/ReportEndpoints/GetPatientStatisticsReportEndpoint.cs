using VaccinationReception.Application.DTOs.Reports;
using VaccinationReception.Application.Reports;

namespace VaccinationReception.API.EndPoints.Reports
{
    public class GetPatientStatisticsReportEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patient-statistics", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                ISender sender) =>
            {
                if ((fromDate.HasValue && toDate.HasValue) && (fromDate > toDate))
                {
                    throw new BadRequestException(BuildingBlocks.Strings.ExceptionKey.FROMDATE_CANNOT_BE_GREATER_THAN_TODATE);
                }

                var query = new GetPatientStatisticsReportQuery(fromDate, toDate);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetPatientStatisticsReport")
            .WithSummary("Get patient statistics report")
            .WithDescription("Retrieve patient statistics by age groups and locations")
            .Produces<PatientStatisticsReportDTO>()
            .ProducesProblem(400)
            .ProducesProblem(500);
        }
    }
}