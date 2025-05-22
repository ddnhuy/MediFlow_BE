namespace Inventory.API.Endpoints
{
    public record UpdateMedicineInteractionResponse(bool IsSuccess);

    public class UpdateMedicineInteractionEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/inventory/medicine-interactions/{id}", async (int id, UpdateMedicineInteractionCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID mismatch between route and body");
                }

                var result = await sender.Send(command);
                var response = result.Adapt<UpdateMedicineInteractionResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateMedicineInteraction")
            .Produces<UpdateMedicineInteractionResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing medicine interaction")
            .WithDescription("Updates an existing medicine interaction record");
        }
    }
}
