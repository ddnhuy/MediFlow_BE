namespace Inventory.API.Endpoints
{
    public record CreateMedicinePriceResponse(int Id);
    public class CreateMedicinePriceEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/medicine-prices", async (CreateMedicinePriceCommand request, ISender sender) =>
            {
                var result = await sender.Send(request);

                var response = new CreateMedicinePriceResponse(result.Id);

                return Results.Created($"/medicine-prices/{result.Id}", response);
            })
            .RequireAuthorization()
            .WithName("CreateMedicinePrice")
            .Produces<CreateMedicinePriceResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Create a new medicine price")
            .WithDescription("Create a new price entry for a specific medicine");
        }
    }
}
