
using Inventory.Application.InventoryLimitStock.Commands;

namespace Inventory.API.Endpoints
{
    public record CreateInventoryLimitStockResponse(bool IsSuccess, int Id);

    public class CreateInventoryLimitStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("inventory-limit-stocks", async (ISender sender, CreateInventoryLimitStockCommand command) =>
            {
                var result = await sender.Send(command);

                var response = result.Adapt<CreateInventoryLimitStockResponse>();

                return Results.Ok(response);

            }).RequireAuthorization()
              .WithName("CreateInventoryLimitStock")
              .Produces<CreateInventoryLimitStockResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create inventory limit stock")
              .WithDescription("Create a new inventory limit stock with the specified medicine ID and minimal stock threshold.");     
        }
    }
}
