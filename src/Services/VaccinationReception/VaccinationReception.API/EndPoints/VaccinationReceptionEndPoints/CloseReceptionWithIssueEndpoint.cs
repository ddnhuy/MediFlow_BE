using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public class CloseReceptionWithIssueEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/closing-reception/reception/{receptionId}", async (int receptionId, ISender sender, CloseReceptionWithIssueCommand command) =>
            {
                var result = await sender.Send(command);

                return Results.Ok(result);
            }).RequireAuthorization()
            .WithName("CloseReceptionWithIssue")
            .WithSummary("Close a vaccination reception with an issue note.")
            .Produces<CloseReceptionWithIssueResult>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithTags("Vaccination Reception Endpoints");
        }
    }
}
