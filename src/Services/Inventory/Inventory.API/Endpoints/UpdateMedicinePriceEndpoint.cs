namespace Inventory.API.Endpoints
{
    public class UpdateMedicinePriceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/medicine-prices/{id}", async (int id, UpdateMedicinePriceCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID in URL does not match ID in request body");
                }

                var result = await sender.Send(command);
                return Results.Ok(result);
            })
            .RequireAuthorization()
            .WithName("UpdateMedicinePrice")
            .Produces<UpdateMedicinePriceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing medicine price")
            .WithDescription("Update the price details for a specific medicine");
        }
    }
}
