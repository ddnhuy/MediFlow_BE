using VaccinationReception.Application.Vaccinations.Commands.RejectVaccination;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class RejectVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/{receptionVaccinationId}/reject",
                async (int receptionVaccinationId, RejectVaccinationCommand request, ISender sender) =>
                {
                    var result = await sender.Send(request);

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("RejectVaccination")
            .WithSummary("Reject a vaccination by setting HasIssue=true")
            .Produces<RejectVaccinationResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags("Vaccination Reception Endpoints");
        }
    }
}