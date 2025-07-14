using Inventory.Application.InventoryLimitStock;

namespace Inventory.API.Endpoints
{
    public record GetInventoryLimitStockByIdResponse(InventoryLimitStockDTO InventoryLimitStock);

    public class GetInventoryLimitStockByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory-limit-stocks/{id}", async (int id, ISender sender) =>
            {
                var result = await sender.Send(new GetInventoryLimitStockByIdQuery(id));
                var response = result.Adapt<GetInventoryLimitStockByIdResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetInventoryLimitStockById")
            .Produces<GetInventoryLimitStockByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get inventory limit stock by Id")
            .WithDescription("Get a single inventory limit stock by its Id.");
        }
    }
}