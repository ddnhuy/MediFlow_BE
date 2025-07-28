using VaccinationReception.Application.Examinations.Queries;
using VaccinationReception.Application.VaccinationReceptions.Queries;

namespace VaccinationReception.API.EndPoints.ExaminationEndpoints
{
    public class GetPatientForExaminationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
           app.MapGet("/examination/patients", async (ISender sender, string? patientName, CancellationToken cancellationToken) =>
           {
                var query = new GetPatientsForExaminationQuery(patientName);
                var response = await sender.Send(query, cancellationToken);
                return Results.Ok(response);
           })
            .RequireAuthorization()
            .WithName("GetPatientsForExamination")
            .Produces<GetPatientsForExaminationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Examinations");
        }
    }
}
