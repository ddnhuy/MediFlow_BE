using VaccinationReception.Application.Patients.Queries;

namespace VaccinationReception.API.EndPoints.PatientEndPoints
{
    public record GeneratePatientIdentifierResponse(string PatientIdentifier);

    public class GeneratePatientIdentifierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/patients/generate-identifier", async (ISender sender) =>
            {
                var query = new GeneratePatientIdentifierQuery();
                var result = await sender.Send(query);

                var response = new GeneratePatientIdentifierResponse(result.PatientIdentifier);
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GeneratePatientIdentifier")
            .WithTags("Patient")
            .Produces<GeneratePatientIdentifierResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithSummary("Generate patient identifier")
            .WithDescription("Generates a unique patient identifier in format CDCDN[YY][MM][DD][HH][MM][SS][mmm]");
        }
    }
}
