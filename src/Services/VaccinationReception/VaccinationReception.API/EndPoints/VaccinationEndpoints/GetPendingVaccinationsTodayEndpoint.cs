using VaccinationReception.Application.Vaccinations.Queries.GetPendingVaccinationsTodayQuery;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public class GetPendingVaccinationsTodayEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/reception/{receptionId}/pending-vaccinations-today", async (int receptionId, ISender sender) =>
            {
                    var query = new GetPendingVaccinationsTodayQuery(receptionId);
                    var result = await sender.Send(query);

                    return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("GetPendingVaccinationsToday")
            .WithSummary("Get pending vaccinations for today")
            .WithDescription("Returns the number of vaccination doses that are scheduled for today but not yet completed")
            .Produces<GetPendingVaccinationsTodayResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Vaccination Reception Endpoints");
        }
    }
}
