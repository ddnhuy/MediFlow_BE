using VaccinationReception.Application.Vaccinations.Commands.CreateVaccination;

namespace VaccinationReception.API.EndPoints.VaccinationEndpoints
{
    public record CreateVaccinationResponse(int VaccinationId);
    public class CreateVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/vaccination", async (CreateVaccinationCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<CreateVaccinationResponse>();
                return Results.Ok(response);
            }).RequireAuthorization()
              .WithName("CreateVaccination")
              .Produces<CreateVaccinationResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Vaccination")
              .WithDescription("Create a new vaccination record in the system. The request body must contain all required fields for the vaccination.");
        }
    }
}
