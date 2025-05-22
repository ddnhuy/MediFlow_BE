namespace Inventory.API.Endpoints
{
    public record DeleteMedicineInteractionResponse(bool IsSuccess);

    public class DeleteMedicineInteractionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/inventory/medicine-interactions/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteMedicineInteractionCommand(id);
                var result = await sender.Send(command);

                var response = result.Adapt<DeleteMedicineInteractionResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteMedicineInteraction")
            .Produces<DeleteMedicineInteractionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a medicine interaction")
            .WithDescription("Soft deletes a medicine interaction by marking it as cancelled");
        }
    }
}
