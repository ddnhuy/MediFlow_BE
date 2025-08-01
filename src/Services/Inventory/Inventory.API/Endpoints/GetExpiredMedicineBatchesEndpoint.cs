namespace Inventory.API.Endpoints
{
    public record GetExpiredMedicineBatchesResponse(PaginatedResult<ExpiredMedicineBatchDto> ExpiredBatches);

    public class GetExpiredMedicineBatchesEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/medicines/expired-batches", async ([AsParameters] PaginationRequest paginationRequest, string? searchTerm, ISender sender) =>
            {
                PaginationHelper.VerifyPaginationRequest(paginationRequest.PageIndex, paginationRequest.PageSize);

                var query = new GetExpiredMedicineBatchesQuery(paginationRequest, searchTerm);
                var result = await sender.Send(query);

                return Results.Ok(new GetExpiredMedicineBatchesResponse(result.ExpiredBatches));
            })
            .RequireAuthorization()
            .WithName("GetExpiredMedicineBatches")
            .Produces<GetExpiredMedicineBatchesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get expired medicine batches")
            .WithDescription("Returns a paginated list of medicine batches that have expired");
        }
    }
}
