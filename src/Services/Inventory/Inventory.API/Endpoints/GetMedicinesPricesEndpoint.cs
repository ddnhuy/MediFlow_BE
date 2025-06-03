namespace Inventory.API.Endpoints
{
    public record GetMedicinePricesResponse(PaginatedResult<MedicinePriceDTO> MedicinePrices);

    public class GetMedicinePricesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicine-prices", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(request.PageIndex, request.PageSize);
                var result = await sender.Send(new GetMedicinePricesQuery(request));
                var response = result.Adapt<GetMedicinePricesResponse>();
                return Results.Ok(response);
            })
            .RequireAuthorization()
            .WithName("GetMedicinePrices")
            .Produces<GetMedicinePricesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get all medicine prices")
            .WithDescription("Get all medicine prices with pagination support");
        }
    }
}
