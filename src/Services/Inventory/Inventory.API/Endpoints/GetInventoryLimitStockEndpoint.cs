using Inventory.Application.InventoryLimitStock;

namespace Inventory.API.Endpoints
{
    public record GetInventoryLimitStockResponse(PaginatedResult<InventoryLimitStockDTO> InventoryLimitStocks);

    public class GetInventoryLimitStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("inventory-limit-stocks", async (ISender sender, [AsParameters] PaginationRequest paginationRequest) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var result = await sender.Send(new GetInventoryLimitStockQuery(paginationRequest));

                var response = result.Adapt<GetInventoryLimitStockResponse>();

                return Results.Ok(result);
            }).RequireAuthorization()
              .WithName("GetInventoryLimitStocks")
              .Produces<GetInventoryLimitStockResponse>(StatusCodes.Status200OK)
              .ProducesProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get inventory limit stocks")
              .WithDescription("Retrieve a paginated list of inventory limit stocks with their current stock and minimal stock threshold.");
        }
    }
}
