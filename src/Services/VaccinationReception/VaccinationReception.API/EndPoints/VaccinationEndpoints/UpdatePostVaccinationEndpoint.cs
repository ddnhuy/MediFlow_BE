using VaccinationReception.Application.Vaccinations.Commands.UpdatePostVaccination;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record UpdatePostVaccinationResponse(bool IsSuccess);

    public class UpdatePostVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/vaccination/{id}/post-vaccination", async (int id, UpdatePostVaccinationCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<UpdatePostVaccinationResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdatePostVaccination")
            .Produces<UpdatePostVaccinationResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update post-vaccination results")
            .WithDescription("Updates the post-vaccination observation results for a specific vaccination");
        }
    }
}
