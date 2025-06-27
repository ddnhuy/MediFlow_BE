
using VaccinationReception.Application.Vaccinations.Queries.GetListPostVaccinationPatient;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record GetListPostVaccinationPatientResponse(List<GetListPostVaccinationPatientQueryResult> Patients);
    public class GetListPostVaccinationPatientEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/vaccination/post-vaccination", async (ISender sender, string? patientVaccinationCode, string? patientName, CancellationToken cancellationToken) =>
            {
                var query = new GetListPostVaccinationPatientQuery(patientVaccinationCode, patientName);
                var result = await sender.Send(query, cancellationToken);
                var response = result.Adapt<GetListPostVaccinationPatientResponse>();
                return Results.Ok(result);
            }).RequireAuthorization()
            .WithName("GetListPostVaccinationPatient")
            .Produces<GetListPostVaccinationPatientResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get list of post-vaccination patients")
            .WithDescription("Retrieves a list of patients who have not confirmed their post-vaccination observation results.");
        }
    }
}
