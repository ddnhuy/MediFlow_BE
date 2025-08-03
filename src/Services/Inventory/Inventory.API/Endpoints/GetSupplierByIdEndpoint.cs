using Inventory.Application.Suppliers.Queries.GetSupplierById;

namespace Inventory.API.Endpoints
{
    public record GetSupplierByIdResponse(SupplierDetailDTO Supplier);

    public class GetSupplierByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/suppliers/{id}", async (int id, ISender sender) =>
            {
                var query = new GetSupplierByIdQuery(id);
                var result = await sender.Send(query);

                var supplierDetailDTO = result.Supplier.Adapt<SupplierDetailDTO>();
                return Results.Ok(new GetSupplierByIdResponse(supplierDetailDTO));
            })
            .RequireAuthorization()
            .WithName("GetSupplierById")
            .Produces<GetSupplierByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get supplier by Id")
            .WithDescription("Get supplier details by Id including contracts");
        }
    }
}