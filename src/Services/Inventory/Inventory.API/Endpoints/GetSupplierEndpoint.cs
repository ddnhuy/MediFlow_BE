namespace Inventory.API.Endpoints
{
    public record GetSupplierResponse(PaginatedResult<SupplierDTO> Suppliers);
    public class GetSupplierEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/suppliers", async([AsParameters] PaginationRequest request, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);
                var result = await sender.Send(new GetSupplierQuery(request));
                var response = result.Adapt<GetSupplierResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetSuppliers")
            .Produces<GetSupplierResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get all suppliers")
            .WithDescription("Get all suppliers with pagination support.");
        }
    }
}
