namespace Inventory.API.Endpoints
{
    public record DeleteMedicineResponse(bool IsSuccess);
    public class DeleteMedicine : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/inventory/medicines/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteMedicineCommand(id);
                var result = await sender.Send(command);

                var response = result.Adapt<DeleteMedicineResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteMedicine")
            .Produces<DeleteMedicineResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a medicine")
            .WithDescription("Soft deletes a medicine by marking it as cancelled");
        }
    }
}
