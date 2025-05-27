namespace Inventory.API.Endpoints
{
    public record UpdateSupplierResponse(bool IsSuccess);
    public class UpdateSupplierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("suppliers/{id}", async (int id, UpdateSupplierCommand command, ISender sender) =>
            {
                if (id != command.Id)
                {
                    return Results.BadRequest("ID mismatch between route and body");
                }

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateSupplierResponse>();

                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("UpdateSupplier")
            .Produces<UpdateSupplierResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update an existing supplier")
            .WithDescription("Updates an existing supplier record in the inventory system");

        }
    }
}
