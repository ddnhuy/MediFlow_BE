namespace Inventory.API.Endpoints
{
    public record GetWarehousesResponse(PaginatedResult<WarehouseDTO> Warehouses);

    public class GettWarehouses : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/inventory/warehouses", async ([FromBody] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetWarehouseQuery(request));
                var response = result.Adapt<GetWarehousesResponse>();
                return Results.Ok(response);
            })
            .WithName("GetWarehouses")
            .Produces<GetWarehousesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all warehouses")
            .WithDescription("Get all warehouses with pagination support.");
        }
    }
}
