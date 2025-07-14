using Inventory.Application.InventoryLimitStock.Commands;

namespace Inventory.API.Endpoints
{
    public record UpdateInventoryLimitStockResponse(bool IsSuccess);

    public class UpdateInventoryLimitStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("inventory-limit-stocks/{id}", async (int id, UpdateInventoryLimitStockCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<UpdateInventoryLimitStockResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateInventoryLimitStock")
            .Produces<UpdateInventoryLimitStockResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update inventory limit stock")
            .WithDescription("Update an existing inventory limit stock with the specified ID and minimal stock threshold.");
        }
    }
}