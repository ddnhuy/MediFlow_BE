using VaccinationReception.Application.VaccinationReceptions.Commands;

namespace VaccinationReception.API.EndPoints.VaccinationReceptionEndPoints
{
    public record CreateReceptionVaccinationResponse(int ReceptionVaccinationId);

    public class CreateReceptionVaccinationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/reception-vaccinations", async (CreateReceptionVaccinationCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                var response = result.Adapt<CreateReceptionVaccinationResponse>();
                return Results.Created($"/reception-vaccinations/{response.ReceptionVaccinationId}", response);
            })
            .RequireAuthorization()
            .WithName("CreateReceptionVaccination")
            .Produces<CreateReceptionVaccinationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create a new reception vaccination")
            .WithDescription("Creates a new vaccination record for a reception");
        }
    }
}