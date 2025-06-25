
using VaccinationReception.Application.Vaccinations.Queries.GetPatientVaccination;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public class GetWaitingVaccinationPatients : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/waiting-patients", async (ISender sender) =>
            {
                var query = new GetPatientVaccinationQuery();
                var results = await sender.Send(query);

                return Results.Ok(results);
            }).RequireAuthorization()
            .Produces<GetPatientVaccinationQueryResult>(StatusCodes.Status200OK)
            .WithName("GetWaitingVaccinationPatients")
            .WithSummary("Get all patients waiting for vaccination")
            .WithDescription("This endpoint retrieves a list of patients who are waiting for vaccination");
        }
    }
}
