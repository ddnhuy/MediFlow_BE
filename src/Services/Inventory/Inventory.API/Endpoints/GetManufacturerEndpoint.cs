namespace Inventory.API.Endpoints
{
    public record GetManufacturersResponse(List<ManufacturerDTO> Manufacturers);
    public class GetManufacturerEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/manufacturers", async (ISender sender) =>
            {
                var result = await sender.Send(new GetManufacturersQuery());
                var response = new GetManufacturersResponse(result.Manufacturers);
                return Results.Ok(response);
            })
             .RequireAuthorization()
             .WithName("GetManufacturers")
             .Produces<GetManufacturersResponse>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound)
             .WithSummary("Get all manufacturers")
             .WithDescription("Returns all active manufacturers");
        }
    }
}
