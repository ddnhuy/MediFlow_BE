namespace Inventory.API.Endpoints
{
    public record CreateMedicineInteractionResponse(int Id);

    public class CreateMedicineInteractionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/inventory/medicine-interactions", async (CreateMedicineInteractionCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return Results.Created($"/inventory/medicine-interactions/{result.Id}", new CreateMedicineInteractionResponse(result.Id));
            })
            .RequireAuthorization()
            .WithName("CreateMedicineInteraction")
            .Produces<CreateMedicineInteractionResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new medicine interaction")
            .WithDescription("Creates a new record of interaction between two medicines");
        }
    }
}
