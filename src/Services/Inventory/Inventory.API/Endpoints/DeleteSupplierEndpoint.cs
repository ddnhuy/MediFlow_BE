namespace Inventory.API.Endpoints
{
    public record DeleteSupplierRespone(bool IsSuccess);
    public class DeleteSupplierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("suppliers/{id}", async (int id, ISender sender) =>
            {
                var command = new DeleteSupplierCommand(id);

                var result = await sender.Send(command);

                var response = result.Adapt<DeleteSupplierRespone>();

                return response;
            })
            .RequireAuthorization()
            .WithName("DeSupplier")
            .Produces<DeleteSupplierRespone>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete supplier")
            .WithDescription("Delete a supplier record in the inventory system");
        }
    }
}
