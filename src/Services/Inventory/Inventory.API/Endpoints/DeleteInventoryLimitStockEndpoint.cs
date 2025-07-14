using Inventory.Application.InventoryLimitStock.Commands;

namespace Inventory.API.Endpoints
{
    public record DeleteInventoryLimitStockResponse(bool IsSuccess);

    public class DeleteInventoryLimitStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("inventory-limit-stocks/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteInventoryLimitStockCommand(id);
                var result = await sender.Send(command);
                var response = result.Adapt<DeleteInventoryLimitStockResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("DeleteInventoryLimitStock")
            .Produces<DeleteInventoryLimitStockResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete inventory limit stock")
            .WithDescription("Soft deletes an inventory limit stock by marking it as cancelled.");
        }
    }
}