namespace Inventory.API.Endpoints
{
    public class DeleteMedicinePriceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/medicine-prices/{id}", async (int id, ISender sender) =>
            {
                if (id <= 0)
                {
                    return Results.BadRequest("ID không hợp lệ");
                }

                var command = new DeleteMedicinePriceCommand(id);
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("DeleteMedicinePrice")
            .Produces<DeleteMedicinePriceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete a medicine price")
            .WithDescription("Mark a specific medicine price as cancelled (soft delete)");
        }
    }
}
