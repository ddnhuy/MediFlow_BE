namespace Inventory.API.Endpoints
{
    public record CreateSupplierResponse(int Id);
    public class CreateSupplierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("suppliers", async (CreateSupplierCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);

                if (result == null)
                {
                    return Results.BadRequest("Failed to create supplier.");
                }

                var response = result.Adapt<CreateSupplierResponse>();
                return Results.Created($"/inventory/suppliers/{response.Id}", response);
            })
            .RequireAuthorization()
            .WithName("CreateSupplier")
            .Produces<CreateSupplierResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create a new supplier")
            .WithDescription("Creates a new supplier in the inventory system");
        }
    }
}
